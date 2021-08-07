using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace MusicDownloader.Core
{
    public static class Downloader
    {
        public static async Task DownloadAsync(AudioFeed audioFeed, bool force = false)
        {
            if (audioFeed == null)
            {
                throw new ArgumentNullException(nameof(audioFeed));
            }

            if (audioFeed.Status == CacheStatus.Downloaded && !force) return;

            // download audios
            var tasks = new List<Task>();
            foreach (var audio in audioFeed.Audios)
            {
                if (audio.Status == CacheStatus.Downloaded && !force) continue;
                
                var link = audio.url.Split('?')[0];

                var process = Process.Start(new ProcessStartInfo
                {
                    FileName = Settings.FfmpegPath,
                    Arguments = $"-y -i {link} {audio.Path}",
                    RedirectStandardError = true // чтобы не захламлять консоль
                });
                var task = process.StandardError.ReadToEndAsync().ContinueWith(t =>
                {
                    process.WaitForExit();
                });
                tasks.Add(task);
            }
            await Task.WhenAll(tasks);
        }
    }
}