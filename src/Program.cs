using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace CSGO_DataLogger
{
    internal static class Program
    {
      private static void Main(string[] args)
      {
          CreateHostBuilder(args).Build().Run();
      }

      private static IWebHostBuilder CreateHostBuilder(string[] args) {
          var config = new ConfigurationBuilder()
              .SetBasePath(Directory.GetCurrentDirectory())
              .AddJsonFile("hostsettings.json", optional: true)
              .AddCommandLine(args)
              .AddEnvironmentVariables("CSGO")
              .Build();
          
          return WebHost.CreateDefaultBuilder(args)
              .UseConfiguration(config)
              .UseStartup<Startup>();
      }
    }
}
