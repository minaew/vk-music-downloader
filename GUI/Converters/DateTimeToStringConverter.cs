using System;
using System.Globalization;

using Avalonia.Data.Converters;

namespace GUI.Converters
{
    internal class DateTimeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var dateTime = (DateTimeOffset)value;

            var date = dateTime.ToString("d");
            var time = dateTime.ToString("t");

            if ((parameter as string) == "time")
            {
                return time;
            }

            return $"{date}{Environment.NewLine}{time}";

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
