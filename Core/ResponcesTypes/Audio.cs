using System.Text.Json.Serialization;

namespace MusicDownloader.Core.ResponcesTypes
{
    public class Audio
    {
        public string artist { get; set; }
        public int id { get; set; }
        public int owner_id { get; set; }
        public string title { get; set; }
        public int duration { get; set; }
        public string access_key { get; set; }
        public string url { get; set; }

        [JsonIgnore]
        public string Path { get; set; }

        [JsonIgnore]
        public CacheStatus Status { get; set; }
    }
}
