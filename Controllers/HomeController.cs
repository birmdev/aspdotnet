using AppASPNETCore.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace AppASPNETCore.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()//Type Interface de ActionResult: type de retour qui permet d'afficher une vue ou de rediriger vers une autre action
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
