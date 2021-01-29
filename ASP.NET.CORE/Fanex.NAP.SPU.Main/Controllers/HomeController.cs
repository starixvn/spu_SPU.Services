namespace Fanex.NAP.SPU.Main.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    
    public class HomeController : Controller
    {

        public IActionResult Index()
        {
            return View();
        }
    }
}
