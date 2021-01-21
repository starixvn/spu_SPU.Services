using Fanex.NAP.SPU.Main.Models;
using Fanex.NAP.SPU.Main.Provider;
using Fanex.NAP.SPU.Main.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Fanex.NAP.SPU.Main.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ISchedulerProvider _schedulerProvider;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            
        }

        public IActionResult Index()
        {
            return View();
        }
       
            //_schedulerProvider.UpdateJobs();
    }
}
