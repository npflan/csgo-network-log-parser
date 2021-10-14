using System.Threading;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;

namespace CSGO_DataLogger.controllers
{
    [ApiController]
    [Route("status")]
    public class StatusController : ControllerBase
    {
        private readonly TaskManager _taskManager;

        public StatusController(IHostedService yolo)
        {
            _taskManager = (TaskManager)yolo;
        }
        
        [HttpGet]
        [Route("start")]
        public async void Start(CancellationToken cancellationToken)
        {
            await _taskManager.StartAsync(cancellationToken);
        }
        
        [HttpGet]
        [Route("stop")]
        public async void Stop(CancellationToken cancellationToken)
        {
            await _taskManager.StopAsync(cancellationToken);
        }
    }
}