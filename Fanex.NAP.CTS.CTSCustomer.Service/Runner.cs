using Fanex.NAP.SPU.Core;
using Microsoft.Owin.Hosting;
using System;
using System.Diagnostics;
using System.IO;
using System.ServiceProcess;
using Fanex.Logging;

namespace Fanex.NAP.CTS.CTSCustomer.Service
{
    public partial class Runner : ServiceBase
    {
        private IDisposable _webApp;

        public Runner()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                LogService.Info($"Customer Transform service is STARTED at {DateTime.Now}");

                _webApp = WebApp.Start<Startup>(AppSettings.HostPathForAccount);
            }
            catch
            {
                var netsh = Path.Combine(
                System.Environment.GetFolderPath(System.Environment.SpecialFolder.System), "netsh.exe");

                var startInfo = new ProcessStartInfo(netsh);
                startInfo.Arguments = "http add urlacl url=" + AppSettings.HostPathForAccount + " user=Everyone listen=yes";
                startInfo.UseShellExecute = true;
                startInfo.Verb = "runas";
                var process = Process.Start(startInfo);
                process.WaitForExit();

                _webApp = WebApp.Start<Startup>(AppSettings.HostPathForAccount);
            }
        }

        protected override void OnStop()
        {
            if (_webApp != null)
            {
                _webApp.Dispose();
            }

            LogService.Info($"Customer Transform service is STOPED at {DateTime.Now}");
        }
    }
}
