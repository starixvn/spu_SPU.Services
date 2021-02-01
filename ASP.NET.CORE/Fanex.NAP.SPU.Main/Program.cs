namespace Fanex.NAP.SPU.Main
{
    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Hosting;
    using Sentry;
    using System;
    using System.IO;

    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args);
            SentrySdk.CaptureMessage(string.Format("{0} has been started at {1}", typeof(Program).FullName, DateTime.Now));
            host.Run();
        }

        public static IHost CreateHostBuilder(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                   .SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                   .Build();


            return Host.CreateDefaultBuilder(args)
                   .ConfigureWebHostDefaults(webBuilder =>
                   {
                       webBuilder                       
                       .UseUrls($"http://*:{configuration["HostingPort"]}")
                       .UseSentry()                       
                       .UseStartup<Startup>();
                   }).Build();
        }        
    }
}
