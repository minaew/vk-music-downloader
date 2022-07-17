using System.Diagnostics;
using System.Text.Json.Serialization;

namespace MusicDownloader.Core.ResponcesTypes
{
    [DebuggerDisplay("{name}")]
    public class Group
    {
        public int id { get; set; }

        public string name { get; set; }

        public string photo_50 { get; set; }

        public string photo_100 { get; set; }

        public string photo_200 { get; set; }

        [JsonIgnore]
        public string Photo { get; set; }
    }
}
