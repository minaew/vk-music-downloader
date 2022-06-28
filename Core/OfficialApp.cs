using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using MusicDownloader.ResponcesTypes;

namespace MusicDownloader.Core
{
    public class OfficialApp
    {
        private readonly string _clientId = 
        // "2274003"
        "2685278"
        ;
        private readonly string _clientSecret = 
        // "hHbZxrka2uZ6jB1inYsH"
        "lxhD8OD7dMsqtXIm5IUY"
        ;
        // private readonly string _deviceId = "5126d15baeff4268";

        public string UserAgent { get; } = 
        // "VKAndroidApp/5.52-4543 (Android 5.1.1; SDK 22; x86_64; unknown Android SDK built for x86_64; en; 320x240)"
        "KateMobileAndroid/89 lite-518 (Android 5.0.2; SDK 21; arm64-v8a; Xiaomi Redmi Note 3; ru)"
        ;

        public string ApiVersion { get; } = 
        // "5.101"
        "5.131"
        ;

        public async Task<string> Authenticate(string userName, string password)
        {
            // scope: notify friends photos audio video docs status notes pages wall groups messages offline notifications stories

            Uri uri = new Uri(
                $"https://oauth.vk.com/token?grant_type=password" +
                $"&client_id={_clientId}" +
                $"&client_secret={_clientSecret}" +
                $"&username={userName}" + 
                $"&password={password}" +
                $"&v={ApiVersion}" + 
                // "&scope=audio,wall,groups" +
                "&scope=notify,friends,photos,audio,video,docs,status,notes,pages,wall,groups,messages,offline,notifications,stories"
                // $"&device_id={_deviceId}"
                );

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("UserAgent", UserAgent);

                var body = await client.GetStringAsync(uri);
                var token = JsonSerializer.Deserialize<Token>(body);
                return token.access_token;
            }
        }
    }
}
