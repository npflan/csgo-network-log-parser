using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace CSGO_DataLogger
{
    public class ConfigCache
    {
        public int ListnerPort { get; private set; } = 9000;
        public bool Debug { get; private set; } = false;
        public List<string> ApiEndpointsToCall { get; private set; } = new List<string>();
        public List<string> ServersFilter { get; private set; } = new List<string>();

        public ConfigCache(IConfiguration _configuration)
        {
            int tempListernetPort;
            if(int.TryParse(_configuration["CSGO_UDP_PORT"], out tempListernetPort))
            {
                ListnerPort = tempListernetPort;
            }
            bool tempDebug;
            if(bool.TryParse(_configuration["CSGO_DebugMode"], out tempDebug))
            {
                Debug = tempDebug;
            }
            foreach(string endpoint in _configuration["CSGO_ApiEndpointsToCall"].Split(';'))
            {
                ApiEndpointsToCall.Add(endpoint);
            }
            foreach (string Server in _configuration["CSGO_ServersToReactTo"].Split(';'))
            {
                ServersFilter.Add(Server);
            }
        }
    }
}