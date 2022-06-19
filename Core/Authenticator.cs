using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using MusicDownloader.ResponcesTypes;

namespace MusicDownloader.Core
{
    public class OfficialApp
    {
        private readonly string _clientId = "2274003";
        private readonly string _clientSecret = "hHbZxrka2uZ6jB1inYsH";
        private readonly string _deviceId = "5126d15baeff4268";

        public string UserAgent { get; } = "VKAndroidApp/5.52-4543 (Android 5.1.1; SDK 22; x86_64; unknown Android SDK built for x86_64; en; 320x240)";

        public async Task<string> Authenticate(string userName, string password)
        {
            Uri uri = new Uri(
                $"https://oauth.vk.com/token?grant_type=password" +
                $"&client_id={_clientId}" +
                $"&client_secret={_clientSecret}" +
                $"&username={userName}" + 
                $"&password={password}" +
                 "&v=5.116" + 
                 "&scope=wall" +
                $"&device_id={_deviceId}");

            string body;
            using (var client = new HttpClient())
            using (var message = new HttpRequestMessage(HttpMethod.Get, uri))
            using (var responce = await client.SendAsync(message))
            {
                responce.EnsureSuccessStatusCode();
                body = await responce.Content.ReadAsStringAsync();
                
            }

            var token = JsonSerializer.Deserialize<Token>(body);
            return token.access_token;
        }
    }
}
