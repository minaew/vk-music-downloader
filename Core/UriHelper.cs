using System;
using System.Collections.Generic;

namespace MusicDownloader.Core
{
    internal static class UriHelper
    {
        private static IReadOnlyDictionary<string, string> GetParams(string uri)
        {
            var segments = uri.Split('?');
            if (segments.Length != 2)
            {
                throw new ArgumentException();
            }

            var dictionary = new Dictionary<string, string>();

            foreach (var parameter in segments[1].Split('&'))
            {
                var keyValue = parameter.Split('=');
                if (keyValue.Length != 2)
                {
                    throw new ArgumentException();
                }
                var key = keyValue[0];
                var value = keyValue[1];

                dictionary[key] = value;
            }

            return dictionary;
        }

        public static string GetParamValue(string uri, string key)
        {
            return GetParams(uri)[key];
        }
    }
}