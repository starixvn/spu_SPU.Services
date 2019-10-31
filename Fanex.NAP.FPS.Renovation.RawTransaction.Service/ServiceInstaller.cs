namespace Fanex.NAP.FPS.Renovation.CapturingData.Service
{
    using System.ComponentModel;
    using System.ServiceProcess;

    [RunInstaller(true)]
    public partial class ServiceInstaller : System.Configuration.Install.Installer
    {
        private readonly ServiceProcessInstaller _process;

        private readonly System.ServiceProcess.ServiceInstaller _service;

        public ServiceInstaller()
        {
            InitializeComponent();
            _process = new ServiceProcessInstaller { Account = ServiceAccount.LocalService };
            _service = new System.ServiceProcess.ServiceInstaller
            {
                ServiceName = "FPS Renovation Capturing Data Service",
                Description = "Processing transaction from RabbitMQ newfpstransQueue"
            };

            Installers.Add(_process);
            Installers.Add(_service);
        }
    }
}
