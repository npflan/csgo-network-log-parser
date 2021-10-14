using System;
using System.Globalization;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CSGO_DataLogger
{
    public class TaskManager : BackgroundService
    {
        private readonly ILogger<TaskManager> _logger;
        private readonly LogManager _logManager;
        private readonly ConfigCache _configCache;

        public TaskManager(ILogger<TaskManager> logger, LogManager logManager, ConfigCache configCache)
        {
            _logger = logger;
            _logManager = logManager;
            _configCache = configCache;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Task start is requested");

            return base.StartAsync(cancellationToken);

        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Task stop is requested");

            return base.StopAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Run(async () =>
            {
                using var udpClient = new UdpClient(_configCache.ListnerPort);
                long counter = 0;
                while (true)
                {
                    if (_configCache.Debug)
                    {
                        _logger.LogInformation($"\rAvailable:{udpClient.Available} Counter:{counter}");
                        counter++;
                    }

                    //waiting for udp package
                    var receivedResults = await udpClient.ReceiveAsync().WithCancellation(stoppingToken);
                                     
                    //Running logic
                    //Task.Run(
                    //    () => _logManager.ParseLog(receivedResults.RemoteEndPoint.Address.ToString(),
                    //        Encoding.ASCII.GetString(receivedResults.Buffer), stoppingToken), stoppingToken);

                    await _logManager.ParseLog(receivedResults.RemoteEndPoint.Address.ToString(),
                            Encoding.ASCII.GetString(receivedResults.Buffer), stoppingToken);
                }
            }, stoppingToken);
        }
    }
}