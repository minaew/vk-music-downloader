using System.Collections.ObjectModel;

namespace MusicDownloader.Core.ResponcesTypes
{
    internal class ResponceCollection<T>
    {
        public int count { get; set; }
        
        public Collection<T> items { get; set; }
    }
}
