﻿using System;
using System.Collections.Generic;

namespace MusicDownloader.Core
{
    public class VkApiException : Exception
    {
        public VkApiException()
        {
        }

        public VkApiException(string message) : base(message)
        {
        }

        public VkApiException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public int Code { get; set; }
        
        public IReadOnlyDictionary<string, string> Parameters { get; set; } = new Dictionary<string, string>();
    }
}
