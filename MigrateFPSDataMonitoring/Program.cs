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
        private static string fileName;
        private static string filePath;

        static void Main(string[] args)
        {
            isRunning = false;

            LogManager.SetDefaultLogCategory("Migrate-FPS-Data-Monitoring")
                    .Use(new List<ILogEngine>
                    {
                         new SentryLogging(new SentryEngineOptions
                         {
                             Dsn = new Dsn("https://0640ef4e164f4a1da89a5183e1600b78@sentry.starixplay.com/123")
                             //Dsn = new Dsn("https://95587e38aa44413094c39b488d4f8425@sentry.nexdev.net/33")
                         })
                    });

            Console.Write("Input process name(ex: notepad): ");
            fileName = Console.ReadLine();

            Console.Write("Input process path(ex: c:/notepad.exe): ");
            filePath = Console.ReadLine();

            StartProcess();

            var timer = new Timer(120000);

            timer.Elapsed += OnTimedEvent;
            timer.AutoReset = true;
            timer.Enabled = true;

            Console.WriteLine("Press the Enter key to exit anytime... ");
            Console.ReadLine();

        }

        private static void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            StartProcess();
        }

        private static void StartProcess()
        {
            if (!Process.GetProcessesByName(fileName).Any())
            {
                try
                {
                    if (isRunning)
                    {
                        isRunning = false;

                        LogService.Error($"{fileName} service is STOPED. Please check....");
                    }

                    using (Process myProcess = new Process())
                    {
                        myProcess.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                        myProcess.StartInfo.FileName = "cmd.exe";
                        myProcess.StartInfo.Arguments = "/c" + filePath;
                        
                        myProcess.Start();

                        isRunning = true;

                       // LogService.Info($"{fileName} service is STARTING at {DateTime.Now}.");
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
