namespace MusicDownloader.Core.ResponcesTypes
{
    internal class Conversations : ResponceCollection<Conversation>
    {
        public int unread_count { get; set; }

        public object profiles {get; set;}

        public object groups {get; set;}
    }

    internal class Conversation
    {

    }
}
