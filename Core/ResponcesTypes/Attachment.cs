namespace MusicDownloader.Core.ResponcesTypes
{
    internal class Attachment
    {
        public string type { get; set; }
        public AudioPlaylist audio_playlist { get; set; }
        public Audio audio { get; set; }
    }
}
