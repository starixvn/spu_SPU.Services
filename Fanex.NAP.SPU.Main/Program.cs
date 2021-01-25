namespace Fanex.NAP.SPU.Main
{
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Hosting;
    using Sentry;
    using System;

    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args);
            SentrySdk.CaptureMessage(string.Format("{0} has been started at {1}", typeof(Program).FullName, DateTime.Now));
            host.Run();
        }

        public static IHost CreateHostBuilder(string[] args) =>
             Host.CreateDefaultBuilder(args)
                 .ConfigureWebHostDefaults(webBuilder =>
                 {
                     webBuilder
                     .UseSentry()
                     .UseStartup<Startup>();
                 }).Build();        
    }
}
