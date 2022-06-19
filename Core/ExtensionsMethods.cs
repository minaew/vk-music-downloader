using System;
using System.Globalization;

namespace MusicDownloader.Core
{
    public static class DateTimeExtensions
    {
        public static string ToUnixTime(this DateTime date)
        {
            var origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            var diff = date - origin;
            var seconds = Math.Floor(diff.TotalSeconds);
            var truncated = (int)seconds;
            return truncated.ToString(CultureInfo.InvariantCulture);
        }
    }
}