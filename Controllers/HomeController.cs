using AppASPNETCore.Models;
using Microsoft.AspNetCore.Authorization; //Pour pouvoir utiliser les attributs [Authorize] et [AllowAnonymous]
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace AppASPNETCore.Controllers
{
    // [Authorize] : toutes les pages de ce contrôleur (Accueil, Privacy) sont réservées aux utilisateurs CONNECTÉS.
    // Une personne non connectée est redirigée vers la page de connexion.
    [Authorize]
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

        // [AllowAnonymous] : EXCEPTION à la règle [Authorize] de la classe.
        // La page d'erreur doit rester visible par tout le monde : si une erreur arrive pendant la connexion,
        // un utilisateur non connecté doit quand même pouvoir voir le message d'erreur.
        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
