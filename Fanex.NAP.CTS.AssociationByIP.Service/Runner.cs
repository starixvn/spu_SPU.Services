using Fanex.Logging;
using Fanex.NAP.SPU.Core;
using Microsoft.Owin.Hosting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.CTS.AssociationByIP.Service
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

            LogService.Info($"Association by IP Transform service is STOPED at {DateTime.Now}");
        }
    }
}
