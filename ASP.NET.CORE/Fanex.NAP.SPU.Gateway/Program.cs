using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Sentry;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Fanex.NAP.SPU.Gateway
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            SentrySdk.CaptureMessage(string.Format("{0} has been started at {1}", typeof(Program).FullName, DateTime.Now));

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) {

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile("App_Data/Job.json", optional: true, reloadOnChange: true)
                .Build();

            return Host.CreateDefaultBuilder(args)
                    .ConfigureAppConfiguration((hostingContext, config) =>
                    {
                        config.Sources.Clear();                      

                        config.AddConfiguration(configuration);    
                    })
                    .ConfigureWebHostDefaults(webBuilder =>
                    {
                        webBuilder
                        .UseSentry()
                        .UseUrls($"http://*:{configuration["HostingPort"]}")
                        .UseStartup<Startup>();
                    });
        }
            
    }
}
