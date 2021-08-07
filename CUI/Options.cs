using System.Collections.Generic;
using CommandLine;

namespace MusicDownloader.CUI
{
#pragma warning disable CA1812 // no instanse
    internal class Options
    {
        [Value(0, HelpText = "interval in days")]
        public int Days { get; set; }

        [Option('d', Default = false, HelpText = "download audios")]
        public bool Download { get; set; }

        [Option('e', "exclude", Separator =',', HelpText = "Ids to exclude")]
        public ICollection<int> IdsToExclude { get; set; }

        [Option('i', "include", HelpText = "Ids to include")]
        public ICollection<int> IdsInclude { get; set; }
    }
#pragma warning restore  CA1812
}
