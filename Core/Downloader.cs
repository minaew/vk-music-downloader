using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace MusicDownloader.Core
{
    public static class Downloader
    {
        public static void Download(AudioFeed audioFeed, bool force = false)
        {
            if (audioFeed == null)
            {
                throw new ArgumentNullException(nameof(audioFeed));
            }

            if (audioFeed.Status == CacheStatus.Downloaded && !force) return;

            // TODO: hz, some heuristic
            var album = audioFeed.Text.Substring(0, 30).Replace("\n", "").Replace("\r", "").Replace("\"", "").Trim();

            // download audios
            var options = new ParallelOptions
            {
                MaxDegreeOfParallelism = Environment.ProcessorCount - 1
            };

            var r = Parallel.For(0, audioFeed.Audios.Count, options, index =>
            {
                var audio = audioFeed.Audios[index];
                if (audio.url.EndsWith(".mp3"))
                {
                    // download
                    using (var client = new HttpClient()) // FIXME: client for each audio
                    using (var stream = client.GetStreamAsync(audio.url).Result)
                    using (var file = File.OpenWrite(audio.Path))
                    {
                        stream.CopyTo(file);
                    }

                    // set metadata
                    var tmpPath = Path.Combine(Path.GetDirectoryName(audio.Path),
                                               $"tmp-{Path.GetFileName(audio.Path)}");
                    foreach (var frame in new Dictionary<string, string>
                    {
                        { "title",     audio.title    },
                        { "performer", audio.artist   },
                        { "album",     album          },
                        { "track",     $"{index + 1}" },
                    })
                    {
                        var dir = Directory.GetCurrentDirectory();
                        var processInfo = new ProcessStartInfo
                        {
                            FileName = Settings.Id3Man,
                            RedirectStandardError = true,
                            RedirectStandardOutput = true,
                        };
                        processInfo.ArgumentList.Add(audio.Path);
                        processInfo.ArgumentList.Add(frame.Key);
                        processInfo.ArgumentList.Add(frame.Value);
                        processInfo.ArgumentList.Add(tmpPath);

                        using (var process = Process.Start(processInfo))
                        {
                            process.WaitForExit();
                            if (process.ExitCode != 0)
                            {
                                var error = process.StandardError.ReadToEndAsync();
                                var output = process.StandardError.ReadToEndAsync();
                                Task.WaitAll(error, output);
                                throw new InvalidOperationException($"error running external process: {error.Result}, output: {output.Result}");
                            }
                        }
                        File.Move(tmpPath, audio.Path, true);
                    }
                }
                else
                {
                    throw new NotImplementedException($"unsupported url {audio.url}");
                }
            });

            if (!r.IsCompleted)
            {
                throw new Exception("Not completed");
            }
        }
    }
}
