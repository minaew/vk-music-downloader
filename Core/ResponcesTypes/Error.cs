using System.Collections.ObjectModel;
using System.Linq;

namespace MusicDownloader.Core.ResponcesTypes
{
    internal class Error
    {
        public int error_code { get; set; }
        public string error_msg { get; set; }
        public Collection<Param> request_params { get; set; }

        public VkApiException ToException()
        {
            return new VkApiException(error_code, request_params.ToDictionary(p => p.key, p => p.value));
        }
    }
}
