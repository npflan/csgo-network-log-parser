using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace CSGO_DataLogger
{
    public class WebCallManager
    {
        private readonly ILogger<WebCallManager> _logger;
        private readonly HttpClient _httpClient;
        private readonly ConfigCache _configCache;

        public WebCallManager(ILogger<WebCallManager> logger, HttpClient httpClient, ConfigCache configCache)
        {
            _logger = logger;
            _httpClient = httpClient;
            _configCache = configCache;
        }

        public async Task MakeWebCall(string serverAction, CancellationToken stoppingToken)
        {
            foreach (string apiEndpoint in _configCache.ApiEndpointsToCall)
            {
                string uri = apiEndpoint.Replace("%%ServerAction%%", serverAction);

                _logger.LogInformation($"Calling: {uri}");
                Task.Run(() => _httpClient.GetAsync(uri, stoppingToken));
            }
        }
    }
}