using System;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace CSGO_DataLogger
{
    public class ConfigCache
    {
        public int ListnerPort { get; private set; } = 9000;
        public bool Debug { get; private set; } = false;
        public List<string> ApiEndpointsToCall { get; private set; } = new List<string>();
        public List<string> ServersFilter { get; private set; } = new List<string>();

        public ConfigCache(IConfiguration configuration)
        {
            if(int.TryParse(configuration["CSGO_UDP_PORT"], out var tempListernetPort))
            {
                ListnerPort = tempListernetPort;
            }

            if(bool.TryParse(configuration["CSGO_DebugMode"], out var tempDebug))
            {
                Debug = tempDebug;
            }
            foreach(string endpoint in configuration["CSGO_ApiEndpointsToCall"].Split(';', StringSplitOptions.RemoveEmptyEntries))
            {
                ApiEndpointsToCall.Add(endpoint);
            }

            if (ApiEndpointsToCall.Count == 0)
            {
                throw new ArgumentException("Environment variable CSGO_ApiEndpointsToCall cannot be empty");
            }

            var ServersToReactTo = configuration["CSGO_ServersToReactTo"];
            if (!string.IsNullOrWhiteSpace(ServersToReactTo))
            {
                foreach (string server in ServersToReactTo.Split(';', StringSplitOptions.RemoveEmptyEntries))
                {
                    ServersFilter.Add(server);
                }
            }
        }
    }
}