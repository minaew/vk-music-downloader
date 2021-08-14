using System.Collections.Generic;
using CommandLine;

namespace MusicDownloader.CUI
{
    internal enum Mode { Post, NewsFeed }

    internal class Options
    {
        [Value(0, HelpText = "mode")]
        public string Mode { get; set; }

        [Value(1, HelpText = "interval in days")]
        public int Days { get; set; }

        [Option('d', Default = false, HelpText = "download audios")]
        public bool Download { get; set; }

        [Option('e', "exclude", Separator =',', HelpText = "Ids to exclude")]
        public ICollection<int> IdsToExclude { get; set; }

        [Option('i', "include", HelpText = "Ids to include")]
        public ICollection<int> IdsInclude { get; set; }

        [Option('g', "groupId", HelpText = "Group identificator")]
        public int GroupId { get; set; }

        [Option('p', "postId", HelpText = "Post identificator")]
        public int PostId { get; set; }
    }
}
