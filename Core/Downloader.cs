using System;
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
                    var processInfo = new ProcessStartInfo
                    {
                        FileName = Settings.FfmpegPath,
                        Arguments = $"-y -i \"{audio.Path}\"" +
                                    $" -metadata title=\"{audio.title}\"" +
                                    $" -metadata artist=\"{audio.artist}\"" +
                                    $" -metadata album=\"{album}\"" +
                                    $" -metadata track=\"{index + 1}\"" +
                                     " -c copy" +
                                    $" {tmpPath}",
                        RedirectStandardError = true // чтобы не захламлять консоль
                    };
                    using (var process = Process.Start(processInfo))
                    // process.StandardError.ReadToEndAsync()
                    //     .ContinueWith(t => 
                    //     {
                    //         process.WaitForExit();
                    //         if (process.ExitCode != 0)
                    //         {
                    //             throw new Exception($"{processInfo.Arguments}: {t.Result}");
                    //         }
                    //     }, TaskScheduler.Current)
                    //     .Wait();
                    {
                        process.WaitForExit();
                    }
                    File.Delete(audio.Path);
                    File.Move(tmpPath, audio.Path);
                }
                else
                {
                    var processInfo = new ProcessStartInfo
                    {
                        FileName = Settings.FfmpegPath,
                        Arguments = $"-y -i \"{audio.url}\"" +
                                    $" -metadata title=\"{audio.title}\"" +
                                    $" -metadata artist=\"{audio.artist}\"" +
                                    $" -metadata album=\"{album}\"" +
                                    $" -metadata track=\"{index + 1}\"" +
                                    $" {audio.Path}",
                        RedirectStandardError = true // чтобы не захламлять консоль
                    };
                    var process = Process.Start(processInfo);
                    process.StandardError.ReadToEndAsync()
                        .ContinueWith(t => 
                        {
                            process.WaitForExit();
                            if (process.ExitCode != 0)
                            {
                                throw new Exception($"{processInfo.Arguments}: {t.Result}");
                            }
                        }, TaskScheduler.Current)
                        .Wait();
                }
            });

            if (!r.IsCompleted)
            {
                throw new Exception("Not completed");
            }
        }
    }
}