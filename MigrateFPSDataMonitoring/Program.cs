using Fanex.Logging;
using Fanex.Logging.Sentry;
using Sentry;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;

using System.Threading.Tasks;
using System.Timers;

namespace MigrateFPSDataMonitoring
{
    class Program
    {
        private static bool isRunning;

        static void Main(string[] args)
        {
            isRunning = false;

            LogManager.SetDefaultLogCategory("Migrate-FPS-Data-Monitoring")
                    .Use(new List<ILogEngine>
                    {
                         new SentryLogging(new SentryEngineOptions
                         {
                             //Dsn = new Dsn("http://5db4409de047476aacba9ee1f3c70e26@10.99.99.37:801/63")
                             Dsn = new Dsn("https://95587e38aa44413094c39b488d4f8425@sentry.nexdev.net/33")
                         })
                    });

            var timer = new Timer(60000);

            timer.Elapsed += OnTimedEvent;
            timer.AutoReset = true;
            timer.Enabled = true;

            Console.WriteLine("Press the Enter key to exit anytime... ");
            Console.ReadLine();

        }

        private static void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            var processName = ConfigurationManager.AppSettings["FileName"];
            
            if (!Process.GetProcessesByName(processName).Any())
            {
                try
                {
                    if (isRunning)
                    {
                        isRunning = false;

                        LogService.Error("Migrate FPS Data service is STOPED. Please check....");
                    }

                    var processPath = ConfigurationManager.AppSettings["FilePath"];

                    using (Process myProcess = new Process())
                    {
                        myProcess.StartInfo.UseShellExecute = false;
                        myProcess.StartInfo.FileName = processPath;
                        myProcess.StartInfo.CreateNoWindow = true;
                        myProcess.Start();

                        isRunning = true;

                        LogService.Info($"Migrate FPS Data service is STARTING at {DateTime.Now}.");
                    }
                }
                catch (Exception ex)
                {
                    LogService.Error(ex.Message);
                }
            }
        }
    }
}
