namespace MusicDownloader.Core.ResponcesTypes
{
    internal class Attachment
    {
        public string type { get; set; }

        // union-ish:
        public AudioPlaylist audio_playlist { get; set; }

        public Audio audio { get; set; }

        public Link link { get; set; }
    }
}
