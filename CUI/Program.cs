using System;
using System.Collections.Generic;
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
            Parser.Default.ParseArguments<Options>(args)
                .WithNotParsed(p => throw new ArgumentException("invalid"))
                .WithParsedAsync(opt =>            
                {
                    switch (opt.Mode)
                    {
                        case "post":
                            return FromPostAsync(opt.GroupId, opt.PostId);

                        case "newsfeed":
                            return FromNewsFeedAsync(opt.Days, opt.Download, opt.IdsToExclude);

                        default:
                            return Task.FromResult(0);
                    }
                })
                .Wait();
        }

        private static async Task FromPostAsync(int groupId, int postId)
        {
            using (var executor = new VkMethodExecutor(FileLogger.Default))
            {
                // var path = await executor.DownloadAudioContenAsync(6827569, 38861);
                var path = await executor.DownloadAudioContenAsync(groupId, postId);
                Console.WriteLine(path);
            }
        }

        private static async Task FromNewsFeedAsync(int days, bool download, ICollection<int> idsToExclude)
        {
            Console.WriteLine($"From {days} ago to now");

            if (download)
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
                await foreach (var feed in executor.GetFeedAsync(DateTime.Now - TimeSpan.FromDays(days),
                                                                DateTime.Now,
                                                                idsToExclude))
                {
                    Console.Write($"{feed.Status}\t{feed.Date}\t{feed.Id}\t{feed.Audios.Count}\t");
                    Console.Write($"{feed.Source.Id}".PadRight(10));
                    Console.WriteLine($"\t{feed.Source.Name}");
                    // post content
                    //foreach (var audio in feed.Audios)
                    //{
                    //    Console.WriteLine($"\t{audio.Status}\t{audio.artist}-{audio.title}");
                    //}
                    if (download)
                    {
                        Console.Write("Downloading ...");
                        var sw = new Stopwatch();
                        sw.Start();
                        Downloader.Download(feed);
                        Console.WriteLine($" in {sw.Elapsed}");
                    }
                }
                // Console.WriteLine($"total time: {swTotal.Elapsed}");
            }

            return;
        }
    }
}