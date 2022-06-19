using System.Collections.ObjectModel;

namespace MusicDownloader.Core.ResponcesTypes
{
    internal class NewsFeed : ResponceCollection<Post>
    {
        public Collection<Group> groups { get; set; }
        public string next_from { get; set; }
    }
}
