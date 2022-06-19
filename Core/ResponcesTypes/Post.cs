using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace MusicDownloader.Core.ResponcesTypes
{
    internal class Post
    {
        public string text { get; set; }
        public int source_id { get; set; }
        public Collection<Attachment> attachments { get; set; } = new Collection<Attachment>();
        public int date { get; set; }
        public int post_id { get; set; }

        public override string ToString()
        {
            return string.Join(',', attachments?.Select(a => a.type) ?? new List<string>());
        }
    }

    internal class WallPost
    {
        public int id { get; set; }
        public int owner_id { get; set; }
        public Collection<Attachment> attachments { get; set; } = new Collection<Attachment>();
        public string text { get; set; }
        public int date { get; set; }
    }
}
