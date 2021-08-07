using System;
using System.IO;
using MusicDownloader.Core;

namespace MusicDownloader.Logger
{
    public class FileLogger : ILogger
    {
        private readonly string _path;

        private FileLogger(string path)
        {
            _path = path;
        }

        public static ILogger Default
        {
            get
            {   
                return new FileLogger(Path.Combine(Settings.ApplicationFolder, "log.txt"));
            }
        }

        public void Log(string messsage)
        {
            File.AppendAllText(_path, $"{DateTime.Now}: {messsage}{Environment.NewLine}");
        }
    }
}