using System;
using System.Text.Json.Serialization;

namespace MusicDownloader.Core.ResponcesTypes
{
    internal class Link
    {
        [JsonPropertyName("url")]
        public string Url { get; set; }
        
        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        // public object Photo { get; set; }
    }
}