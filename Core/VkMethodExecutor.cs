using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using MusicDownloader.Core.ResponcesTypes;

namespace MusicDownloader.Core
{
    public sealed class VkMethodExecutor : IDisposable
    {
        #region Fields

        private readonly ILogger _logger;
        private readonly HttpClient _client;
        private string _token;

        #endregion

        public VkMethodExecutor(ILogger logger)
        {
            string userAgent;
            using (var file = new FileStream(Settings.TokenPath, FileMode.Open))
            using (var stream = new StreamReader(file))
            {
                _token = stream.ReadLine();
                userAgent = stream.ReadLine();
            }

            _logger = logger;
            _client = new HttpClient();
            _client.DefaultRequestHeaders.Add("User-Agent", userAgent);
        }

        #region Public

        public void Dispose()
        {
            _client?.Dispose();
        }

        public async IAsyncEnumerable<AudioFeed> GetFeedAsync(DateTime start, DateTime end, ICollection<int> excludeSources = null)
        {
            string startFrom = null;
            PartialFeed part;
            while (true)
            {
                try
                {
                    part = await GetFeedAsync(start, end, startFrom);
                    startFrom = part.StartFrom;
                }
                catch (AggregateException ex) when (ex.InnerException is VkApiException vkex)
                {
                    var message = "api exception occured: " + vkex.Message + " " +
                                  string.Join(",", vkex.Parameters.Select(p => p.Key + ":" + p.Value));
                    throw new Exception(message);
                }

                foreach (var feed in part)
                {
                    if (excludeSources?.Contains(feed.Source.Id) ?? false)
                    {
                        continue;
                    }

                    yield return feed;
                }

                if (startFrom == null)
                {
                    break;
                }
            }
        }

        public async Task<string> DownloadAudioContenAsync(int groupId, int postId)
        {
            var posts = await ExecuteAsync<WallPost[]>("wall.getById", new Dictionary<string, string>
            {
                {"posts", $"-{groupId}_{postId}"}
            });

            var post = posts.SingleOrDefault();
            var audioFeed = await ProcessPostAsync(post);
            await Downloader.DownloadAsync(audioFeed, true);
            _logger.Log($"donloaded in {audioFeed.Location}");
            return audioFeed.Location.FullName;
        }

        #endregion

        #region Private

        private async Task<AudioFeed> ProcessPostAsync(WallPost post)
        {
            var audios = new List<Audio>();
            foreach (var att in post.attachments)
            {
                switch (att.type)
                {
                    case "audio":
                        audios.Add(att.audio);
                        break;
                    
                    case "audio_playlist":
                        var playList = await ExecuteAsync<ResponceCollection<Audio>>("audio.get", new Dictionary<string, string>
                        {
                            { "owner_id", att.audio_playlist.owner_id.ToString(CultureInfo.InvariantCulture) },
                            { "playlist_id", att.audio_playlist.id.ToString(CultureInfo.InvariantCulture) },
                        });
                        if (playList != null)
                        {
                            audios.AddRange(playList.items);
                        }
                        break;
                }
            }

            var group = new Group() { Id = -post.owner_id };
            var date = DateTimeOffset.FromUnixTimeSeconds(post.date).ToOffset(new TimeSpan(3, 0, 0));
            return new AudioFeed(post.id, group, date, audios)
            {
                Text = post.text
            };
        }
        
        private async Task<PartialFeed> GetFeedAsync(DateTime start, DateTime end, string startFrom)
        {
            var parameters = new Dictionary<string, string>
            {
                {"filters", "post"},
                {"start_time", start.ToUnixTime()},
                {"end_time", end.ToUnixTime()},
            };
            if (startFrom != null)
            {
                parameters.Add("start_from", startFrom);
            }

            _logger.Log($"Getting feed from {start} to {end}, start_from={startFrom}");

            var newsFeed = await ExecuteAsync<NewsFeed>("newsfeed.get", parameters);

            _logger.Log($"Got {newsFeed.items.Count} items");

            var feeds = new HashSet<AudioFeed>(AudioFeedComparer.Instance);
            foreach (var item in newsFeed.items)
            {
                if (item.source_id > 0) continue; // not a group
                
                var group = newsFeed.groups.SingleOrDefault(g => g.Id == - item.source_id);
                if (group == null) continue; // unknown group

                await DownloadOrSetPhotoAsync(group);

                var audios = new List<Audio>();
                foreach (var att in item.attachments)
                {
                    switch (att.type)
                    {
                        case "audio":
                            audios.Add(att.audio);
                            break;
                        
                        case "audio_playlist":
                            var playList = await ExecuteAsync<ResponceCollection<Audio>>("audio.get", new Dictionary<string, string>
                            {
                                { "owner_id", att.audio_playlist.owner_id.ToString(CultureInfo.InvariantCulture) },
                                { "playlist_id", att.audio_playlist.id.ToString(CultureInfo.InvariantCulture) },
                            });
                            if (playList != null)
                            {
                                audios.AddRange(playList.items);
                            }
                            break;
                    }
                }
                
                if (audios.Count > 0)
                {
                    feeds.Add(new AudioFeed(item.post_id,
                                            group,
                        DateTimeOffset.FromUnixTimeSeconds(item.date).ToOffset(new TimeSpan(3, 0, 0)),
                                            audios)
                    {
                        Text = item.text
                    });
                }
            }

            return new PartialFeed(feeds, newsFeed.next_from);
        }

        private async Task<T> ExecuteAsync<T>(string method, IDictionary<string, string> parameters)
        {
            var body = await ExecuteAsync(method, parameters);
            if (string.IsNullOrEmpty(body))
            {
                return default;
            }
            var deserializedBody = JsonSerializer.Deserialize<GenericResponce<T>>(body);
            return deserializedBody.response;
        }
        
        private async Task<string> ExecuteAsync(string method, IDictionary<string, string> parameters)
        {
            var requestUri = new Uri(
                    "https://api.vk.com/method/" + method + "?" +
                    string.Concat(parameters.Select(p => $"&{p.Key}={p.Value}")) +
                    "&access_token=" + _token +
                    // "&v=5.122"
                    "&v=5.101"
                );

            string body;
            using (var message = new HttpRequestMessage(HttpMethod.Get, requestUri))
            using (var response = await _client.SendAsync(message))
            {
                response.EnsureSuccessStatusCode();
                body = await response.Content.ReadAsStringAsync();
            }

            var error = JsonSerializer.Deserialize<ErrorResponce>(body);
            if (error.error != null)
            {
                switch (error.error.error_code)
                {
                    case 6: // to many requests
                        await Task.Delay(100);
                        return await ExecuteAsync(method, parameters);

                    case 15:
                        _logger.Log("error 15: " + string.Join(',', error.error.request_params.Select(p => $"{p.key}:{p.value}")));
                        return null;

                    case 201: // captcha
                        _logger.Log("error 201");
                        return null;

                    case 14: // captcha
                        _logger.Log("error 14");
                        return null;

                    default:
                        throw error.error.ToException();
                }
            }

            return body;
        }

        private async Task DownloadOrSetPhotoAsync(Group group)
        {
            var groupsCache = Path.Combine(Settings.ApplicationFolder, "groups");

            var extension = group.Photo50?.Split('?')[0].Split('.').LastOrDefault();
            group.Photo = Path.Combine(groupsCache, $"{group.Id}_50.{extension}");
            if (File.Exists(group.Photo))
            {
                return;
            }

            var imageBytes = await _client.GetByteArrayAsync(group.Photo200);

            Directory.CreateDirectory(groupsCache);
            await File.WriteAllBytesAsync(group.Photo, imageBytes);

        }

        #endregion
    }
}
