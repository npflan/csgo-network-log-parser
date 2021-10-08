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
        private readonly IConfiguration _configuration;

        public TaskManager(ILogger<TaskManager> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration.GetSection("CSGO");
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
                var port = int.Parse(_configuration["UDP_PORT"], NumberStyles.Integer);

                using var udpClient = new UdpClient(port);
                long counter = 0;
                while (true)
                {
                    _logger.LogInformation($"\rAvailable:{udpClient.Available} Counter:{counter}");
                    //if (udpClient.Available > 0)
                    //WriteLine($"Socket data available {udpClient.Available}");
                    var receivedResults = await udpClient.ReceiveAsync().WithCancellation(stoppingToken);
                    counter++;
                    _logger.LogInformation("derp");
                    Task.Run(
                        () => LogManager.ParseLog(receivedResults.RemoteEndPoint.Address.ToString(),
                            Encoding.ASCII.GetString(receivedResults.Buffer)), stoppingToken);
                    _logger.LogInformation("derp2");

                    //ClassLibrary_ParseMessage.Manager.ParseLog(receivedResults.RemoteEndPoint.Address.ToString(), Encoding.ASCII.GetString(receivedResults.Buffer));
                }
            }, stoppingToken);
        }
    }
}