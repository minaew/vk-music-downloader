using System;
using System.Collections.Generic;

namespace MusicDownloader.Core
{
    public class VkApiException : Exception
    {
        public VkApiException(int code, IReadOnlyDictionary<string, string> parameters)
        {
            Code = code;
            Parameters = parameters;
        }

        public VkApiException()
        {
        }

        public VkApiException(string message) : base(message)
        {
        }

        public VkApiException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public int Code { get; }
        
        public IReadOnlyDictionary<string, string> Parameters { get; }
    }
}