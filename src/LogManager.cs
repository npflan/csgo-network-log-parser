using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace CSGO_DataLogger
{
    public class LogManager
    {
        private readonly ILogger<LogManager> _logger;
        private readonly WebCallManager _webCallManager;
        private readonly ConfigCache _configCache;

        public LogManager(ILogger<LogManager> logger, WebCallManager webCallManager, ConfigCache configCache)
        {
            _logger = logger;
            _webCallManager = webCallManager;
            _configCache = configCache;
        }

        private Dictionary<string, string> _regexHash = new Dictionary<string, string>() {
                       //"(?<PlayerName>.*)<[0-9]*><(?<PlayerID>STEAM_[0-9]*:[0-9]*:[0-9]*|BOT)><(?<Team>TERRORIST)>"\striggered\s"(?<EventID>Planted_The_Bomb*)"(\s\(value\s"(?<Value>.*)"\))?
            { "BombPlanted", "\"(?<PlayerName>.*)<[0-9]*><(?<PlayerID>STEAM_[0-9]*:[0-9]*:[0-9]*|BOT)><(?<Team>TERRORIST)>\"\\striggered\\s\"(?<EventID>Planted_The_Bomb*)\"(\\s\\(value\\s\"(?<Value>.*)\"\\))?" },

            //Team\s"(?<Team>TERRORIST)"\striggered\s"(?<Event>SFUI_Notice_Terrorists_Win)"
            { "TerroristsWin", "Team\\s\"(?<Team>TERRORIST)\"\\striggered\\s\"(?<Event>SFUI_Notice_Terrorists_Win)\"" },
            
            //Team\s"(?<Team>TERRORIST)"\striggered\s"(?<Event>SFUI_Notice_Target_Bombed)"
            { "BombExploded", "Team\\s\"(?<Team>TERRORIST)\"\\striggered\\s\"(?<Event>SFUI_Notice_Target_Bombed)\"" },

            //Team\s"(?<Team>CT)"\striggered\s"(?<Event>SFUI_Notice_CTs_Win)"
            { "CTsWin", "Team\\s\"(?<Team>CT)\"\\striggered\\s\"(?<Event>SFUI_Notice_CTs_Win)\"" },

            //Team\s"(?<Team>CT)"\striggered\s"(?<Event>SFUI_Notice_Bomb_Defused)"
            { "BombDefused", "Team\\s\"(?<Team>CT)\"\\striggered\\s\"(?<Event>SFUI_Notice_Bomb_Defused)\"" },

            //World\striggered\s"(?<Type>Round_Start*)"
            { "RoundStart", "World\\striggered\\s\"(?<Type>Round_Start*)\"" },
        };

        public async Task ParseLog(string server, string log, CancellationToken stoppingToken)
        {
            if (_configCache.Debug)
            {
                _logger.LogInformation($"{server} Sended: {log}");
            }

            if(!_configCache.ServersFilter.Contains(server) && _configCache.ServersFilter.Count != 0)
            {
                _logger.LogInformation($"{server} message was dropped");
                return;
            }

            string serverAction = "";
            foreach (var regex in _regexHash)
            {
                var match = Regex.Match(log, regex.Value);
                if (!match.Success)
                {
                    continue;
                }

                serverAction = regex.Key;
            }

            if(string.IsNullOrWhiteSpace(serverAction))
            {
                if(_configCache.Debug)
                {
                    _logger.LogInformation($"not match found for {log}");
                }
                return;
            }

            await _webCallManager.MakeWebCall(serverAction: serverAction, stoppingToken);
        }
    }
}