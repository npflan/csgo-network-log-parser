using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CSGO_DataLogger.controllers
{
    [ApiController]
    [Route("Log")]
    public class LogController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly ConfigCache _configCache;
        private readonly LogManager _logManager;

        public LogController(ILogger<LogController> logger,ConfigCache configCache, LogManager logManager)
        {
            _logger = logger;
            _configCache = configCache;
            _logManager = logManager;
        }

        [HttpPost]
        [Route("Log")]
        //[ReadableBodyStream]
        public async Task Log(CancellationToken cancellationToken)
        {
            string lines = await StreamToStringAsync(Request);
            foreach (string line in lines.Split('\n'))
            {
                if (_configCache.Debug)
                {
                    _logger.LogInformation($"Recieved: {line}");
                    Task.Run(() => _logManager.ParseLog(Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown", line, cancellationToken));
                }
            }
            }

        private async Task<string> StreamToStringAsync(HttpRequest request)
        {
            using (var sr = new StreamReader(request.Body))
            {
                return await sr.ReadToEndAsync();
            }
        }
    }
}