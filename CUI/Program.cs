using System;
using System.Diagnostics;
using System.Threading.Tasks;
using CommandLine;
using MusicDownloader.Core;
using MusicDownloader.Logger;

namespace MusicDownloader.CUI
{
    class Program
    {
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args).WithParsedAsync(OperateAsync).Wait();
        }

        private static async Task OperateAsync(Options options)
        {
            using (var executor = new VkMethodExecutor(FileLogger.Default))
            {
                var path = await executor.DownloadAudioContenAsync(6827569, 38861);
                Console.WriteLine(path);
            }

            return;

            Console.WriteLine($"From {options.Days} ago to now");

            if (options.Download)
            {
                Console.WriteLine("With download");
            }
            else
            {
                Console.WriteLine("Without download");
            }

            using (var executor = new VkMethodExecutor(FileLogger.Default))
            {
                Console.WriteLine("Getting feeds ...");
                await foreach (var feed in executor.GetFeedAsync(DateTime.Now - TimeSpan.FromDays(options.Days),
                                                                DateTime.Now,
                                                                options.IdsToExclude))
                {
                    Console.Write($"{feed.Status}\t{feed.Date}\t{feed.Id}\t{feed.Audios.Count}\t");
                    Console.Write($"{feed.Source.Id}".PadRight(10));
                    Console.WriteLine($"\t{feed.Source.Name}");
                    // post content
                    //foreach (var audio in feed.Audios)
                    //{
                    //    Console.WriteLine($"\t{audio.Status}\t{audio.artist}-{audio.title}");
                    //}
                    if (options.Download)
                    {
                        Console.Write("Downloading ...");
                        var sw = new Stopwatch();
                        sw.Start();
                        await Downloader.DownloadAsync(feed);
                        Console.WriteLine($" in {sw.Elapsed}");
                    }
                }
                // Console.WriteLine($"total time: {swTotal.Elapsed}");
            }

            return;
        }
    }
}