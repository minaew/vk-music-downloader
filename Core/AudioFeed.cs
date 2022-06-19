using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using MusicDownloader.Core.ResponcesTypes;

namespace MusicDownloader.Core
{
    [DebuggerDisplay("{Date} {Source.Name}")]
    public class AudioFeed
    {
        public AudioFeed(int id, Group source, DateTimeOffset date, IEnumerable<Audio> audios = null)
        {
            Id = id;
            Source = source;
            Date = date;

            Location = Directory.CreateDirectory(Path.Combine(Settings.ApplicationFolder,
                                                              "cache",
                                                              Source.Id.ToString(CultureInfo.InvariantCulture),
                                                              Id.ToString(CultureInfo.InvariantCulture)));

            if (audios != null)
            {
                Audios.AddRange(audios);
            }

            foreach (var audio in Audios)
            {
                audio.Path = Path.Combine(Location.FullName, $"{audio.id}.mp3");
                audio.Status = File.Exists(audio.Path) ? CacheStatus.Downloaded : CacheStatus.NotDownloaded;
            }

            if (Audios.Any(a => a.Status == CacheStatus.Downloaded))
            {
                Status = CacheStatus.PartialDownloaded;
            }
            if (Audios.All(a => a.Status == CacheStatus.Downloaded))
            {
                Status = CacheStatus.Downloaded;
            }
        }

        public int Id { get; }

        public Group Source { get; }

        public string Text { get; set; }
        
        public DateTimeOffset Date { get; }

        public IList<Audio> Audios { get; } = new List<Audio>();

        public DirectoryInfo Location { get; }

        public CacheStatus Status { get; }
    }

    public class AudioFeedComparer : IEqualityComparer<AudioFeed>
    {
        public static AudioFeedComparer Instance { get; } = new AudioFeedComparer();

        public bool Equals(AudioFeed x, AudioFeed y)
        {
            if ((x == null) && (y == null))
            {
                return true;
            }

            if ((x == null) || (y == null))
            {
                return false;
            }

            return x.Id == y.Id && x.Source.Id == y.Source.Id;
        }

        public int GetHashCode(AudioFeed obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            return obj.Id.GetHashCode() ^ obj.Source.Id.GetHashCode();
        }
    }
}