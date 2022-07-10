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

        public static string Id3Man { get; } = "ID3Man";
    }
}