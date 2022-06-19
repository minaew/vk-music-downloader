using System.Collections.Generic;

namespace MusicDownloader.Core.ResponcesTypes
{
    internal class MessageHistory : ResponceCollection<Message>
    {
    }

    internal class Message
    {
        public string text { get; set; }

        public ICollection<Attachment> attachments { get; set; }
    }
}
