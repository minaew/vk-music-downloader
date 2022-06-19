namespace MusicDownloader.ResponcesTypes
{
    internal class Token
    {
        public string access_token { get; set; }

        public int expires_in { get; set; }

        public int user_id { get; set; }

        public string webview_refresh_token { get; set; }

        public string webview_access_token { get; set; }

        public string webview_access_token_expires_in { get; set; }
    }
}
