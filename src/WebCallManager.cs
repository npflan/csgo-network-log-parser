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

        public async Task MakeWebCall(string ServerAction, CancellationToken stoppingToken)
        {
            foreach (string ApiEndpoint in _configCache.ApiEndpointsToCall)
            {
                string uri = ApiEndpoint.Replace("%%ServerAction%%", ServerAction);

                if(_configCache.Debug)
                {
                    _logger.LogInformation($"Now calling: {uri}");
                }

                await _httpClient.GetAsync(uri, stoppingToken);
            }
        }
    }
}