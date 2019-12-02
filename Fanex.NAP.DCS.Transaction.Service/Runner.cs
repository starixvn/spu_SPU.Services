namespace Fanex.NAP.DCS.Transaction.Service
{
    using Fanex.Logging;
    using Fanex.NAP.DCS.Core;
    using Fanex.NAP.SPU.Core;
    using Microsoft.Owin.Hosting;
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.ServiceProcess;

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
                _webApp = WebApp.Start<Startup>(AppSettings.HostPathForTransaction);
            }
            catch
            {
                var netsh = Path.Combine(
                System.Environment.GetFolderPath(System.Environment.SpecialFolder.System), "netsh.exe");
                                
                var startInfo = new ProcessStartInfo(netsh);
                startInfo.Arguments = "http add urlacl url="+ AppSettings.HostPathForTransaction + " user=Everyone listen=yes";
                startInfo.UseShellExecute = true;
                startInfo.Verb = "runas";
                var process = Process.Start(startInfo);
                process.WaitForExit();

                _webApp = WebApp.Start<Startup>(AppSettings.HostPathForTransaction);
            }
        }

        protected override void OnStop()
        {
            if (_webApp != null)
            {
                _webApp.Dispose();
            }

            LogService.Info($"Data Transform service is STOPED at {DateTime.Now}");
        }
    }
}
