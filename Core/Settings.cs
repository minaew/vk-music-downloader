using System;
using System.IO;

namespace MusicDownloader.Core
{
    public static class Settings
    {
        public static string ApplicationFolder { get; } =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                         "MusicDownloader");

        public static string TokenPath { get; } = Path.Combine(ApplicationFolder, "android-token.txt");

        public static string FfmpegPath
        {
            get
            {
                switch (Environment.OSVersion.Platform)
                {
                    case PlatformID.Win32NT:
                        return @"F:\Fracture\Development\sources\vk\ffmpeg-4.3.1-2020-10-01-full_build\bin\ffmpeg.exe";

                    case PlatformID.Unix:
                        return "ffmpeg";

                    default:
                        throw new NotImplementedException();
                }
            }
        }
    }
}