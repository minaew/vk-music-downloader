using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using MusicDownloader.ResponcesTypes;

namespace MusicDownloader.Core
{
    public class VkApplication
    {
        private readonly string _clientId = "2685278";
        private readonly string _clientSecret = "lxhD8OD7dMsqtXIm5IUY";
        public string UserAgent { get; } = "KateMobileAndroid/89 lite-518 (Android 5.0.2; SDK 21; arm64-v8a; Xiaomi Redmi Note 3; ru)";

        public string ApiVersion { get; } = "5.131";

        public async Task<string> Authenticate(string userName, string password)
        {
            var uri = $"https://oauth.vk.com/token?grant_type=password" +
                      $"&client_id={_clientId}" +
                      $"&client_secret={_clientSecret}" +
                      $"&username={userName}" + 
                      $"&password={password}" +
                      $"&v={ApiVersion}" + 
                      "&scope=notify,friends,photos,audio,video,docs,status,notes,pages,wall,groups,messages,offline,notifications,stories";

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("UserAgent", UserAgent);

                var body = await client.GetStringAsync(uri);
                var token = JsonSerializer.Deserialize<Token>(body);
                return token.access_token;
            }
        }
    }
}
