using System;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace CSGO_DataLogger
{
    public class ConfigCache
    {
        private readonly ILogger<ConfigCache> _logger;

        public int ListnerPort { get; private set; } = 9000;
        public bool Debug { get; private set; } = false;
        public List<string> ApiEndpointsToCall { get; private set; } = new List<string>();
        public List<string> ServersFilter { get; private set; } = new List<string>();

        public ConfigCache(ILogger<ConfigCache> logger, IConfiguration configuration)
        {
            _logger = logger;

            if (int.TryParse(configuration["CSGO_UDP_PORT"], out var tempListernetPort))
            {
                ListnerPort = tempListernetPort;
            }

            _logger.LogInformation($"ListnerPort: {ListnerPort}");

            if(bool.TryParse(configuration["CSGO_DebugMode"], out var tempDebug))
            {
                Debug = tempDebug;
            }

            _logger.LogInformation($"Debug: {Debug}");

            foreach (string endpoint in configuration["CSGO_ApiEndpointsToCall"].Split(';', StringSplitOptions.RemoveEmptyEntries))
            {
                ApiEndpointsToCall.Add(endpoint);
                _logger.LogInformation($"ApiEndpointsToCall: {endpoint}");
            }
            
            if (ApiEndpointsToCall.Count == 0)
            {
                _logger.LogInformation("Environment variable CSGO_ApiEndpointsToCall cannot be empty");
                throw new ArgumentException("Environment variable CSGO_ApiEndpointsToCall cannot be empty");
            }

            var ServersToReactTo = configuration["CSGO_ServersToReactTo"];
            if (!string.IsNullOrWhiteSpace(ServersToReactTo))
            {
                foreach (string server in ServersToReactTo.Split(';', StringSplitOptions.RemoveEmptyEntries))
                {
                    ServersFilter.Add(server);
                    _logger.LogInformation($"ServersFilter: {server}");
                }
            }

            
        }
    }
}