using System;
using System.Diagnostics;
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
            });

            if (!r.IsCompleted)
            {
                throw new Exception("Not completed");
            }
        }
    }
}