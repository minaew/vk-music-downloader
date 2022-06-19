using System.Diagnostics;
using System.Text.Json.Serialization;

namespace MusicDownloader.Core.ResponcesTypes
{
    [DebuggerDisplay("{name}")]
    public class Group
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("photo_50")]
        public string Photo50 { get; set; }

        [JsonPropertyName("photo_100")]
        public string Photo100 { get; set; }

        [JsonPropertyName("photo_200")]
        public string Photo200 { get; set; }

        [JsonIgnore]
        public string Photo { get; set; }
    }
}
