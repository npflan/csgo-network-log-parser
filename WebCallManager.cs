using System.Net.Http;

namespace CSGO_DataLogger
{
    public static class WebCallManager
    {
        public static void MakeWebCall(string uri)
        {
            var client = new HttpClient();
            client.GetAsync(uri);
        }
    }
}