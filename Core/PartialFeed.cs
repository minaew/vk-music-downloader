using System.Collections;
using System.Collections.Generic;

namespace MusicDownloader.Core
{
    internal class PartialFeed : IEnumerable<AudioFeed>
    {
        public List<AudioFeed> _posts;

        public PartialFeed(IEnumerable<AudioFeed> posts, string startFrom)
        {
            _posts = new List<AudioFeed>(posts);
            StartFrom = startFrom;
        }        

        public string StartFrom { get; }

        public IEnumerator<AudioFeed> GetEnumerator()
        {
            return _posts.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_posts).GetEnumerator();
        }
    }
}