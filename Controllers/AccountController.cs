using AppASPNETCore.Models.ViewModels; //Pour pouvoir utiliser LoginViewModel et RegisterViewModel
using Microsoft.AspNetCore.Identity; //Pour pouvoir utiliser UserManager, SignInManager et IdentityUser
using Microsoft.AspNetCore.Mvc; //Pour pouvoir utiliser Controller, IActionResult, [HttpPost], etc.

namespace AppASPNETCore.Controllers
{
    // ============================================================================================
    // AccountController : gère tout ce qui concerne le COMPTE de l'utilisateur.
    //   /Account/Register -> inscription (création d'un compte)
    //   /Account/Login    -> connexion
    //   /Account/Logout   -> déconnexion
    //
    // IMPORTANT : contrairement à HomeController et StudentsController, ce contrôleur n'a PAS l'attribut [Authorize].
    // C'est normal : les pages de connexion et d'inscription doivent être accessibles aux personnes
    // qui ne sont PAS encore connectées (sinon personne ne pourrait jamais se connecter !).
    //
    // Ce contrôleur utilise ASP.NET Core Identity, le système officiel de Microsoft pour gérer les utilisateurs.
    // Identity s'occupe des parties compliquées et sensibles à notre place :
    //   - le mot de passe n'est JAMAIS enregistré en clair : il est "haché" (transformé de façon irréversible) ;
    //   - la vérification du mot de passe lors de la connexion ;
    //   - la création du cookie de connexion (le petit fichier qui permet au navigateur de rester connecté).
    // ============================================================================================
    public class AccountController : Controller
    {
        // UserManager : sert à GÉRER les utilisateurs (créer un compte, chercher un utilisateur...).
        private readonly UserManager<IdentityUser> _userManager;

        // SignInManager : sert à CONNECTER et DÉCONNECTER les utilisateurs (vérifier le mot de passe, créer/supprimer le cookie).
        private readonly SignInManager<IdentityUser> _signInManager;

        // ----------------------------------------------------------------------------------------
        // CONSTRUCTEUR : INJECTION DE DÉPENDANCES
        // Comme pour IStudentService, on ne crée pas ces objets avec "new" :
        // ASP.NET nous les donne automatiquement, car ils ont été enregistrés dans Program.cs
        // avec builder.Services.AddIdentity<IdentityUser, IdentityRole>()...
        // ----------------------------------------------------------------------------------------
        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // ========================================================================================
        // INSCRIPTION - ÉTAPE 1 : afficher le formulaire d'inscription VIDE
        // URL : GET /Account/Register
        // ========================================================================================
        public IActionResult Register()
        {
            return View(); //Affiche Views/Account/Register.cshtml
        }

        // ========================================================================================
        // INSCRIPTION - ÉTAPE 2 : recevoir le formulaire et CRÉER le compte
        // URL : POST /Account/Register
        // ========================================================================================
        [HttpPost] //Uniquement pour les requêtes POST (envoi du formulaire)
        [ValidateAntiForgeryToken] //Sécurité anti-CSRF : vérifie que le formulaire vient bien de notre site
        public async Task<IActionResult> Register(RegisterViewModel model) //"model" est rempli automatiquement avec les champs du formulaire
        {
            // On vérifie les règles écrites dans RegisterViewModel ([Required], [EmailAddress], [Compare]...)
            if (!ModelState.IsValid)
            {
                return View(model); //Données incorrectes : on réaffiche le formulaire avec les messages d'erreur
            }

            // On prépare le nouvel utilisateur.
            // Identity a besoin d'un "UserName" (nom d'utilisateur) : on utilise simplement l'email.
            IdentityUser user = new IdentityUser
            {
                UserName = model.Email,
                Email = model.Email
            };

            // CreateAsync() crée le compte dans la base de données (table AspNetUsers).
            // On lui donne le mot de passe EN CLAIR : c'est Identity qui le hache avant de l'enregistrer.
            // Identity vérifie aussi que le mot de passe est assez fort (règles définies dans Program.cs)
            // et que l'email n'est pas déjà utilisé par un autre compte.
            IdentityResult result = await _userManager.CreateAsync(user, model.Password);

            // result.Succeeded vaut true si le compte a bien été créé
            if (result.Succeeded)
            {
                // On connecte directement le nouvel utilisateur (pour qu'il n'ait pas à se reconnecter juste après).
                // isPersistent: false -> le cookie de connexion sera supprimé à la fermeture du navigateur.
                await _signInManager.SignInAsync(user, isPersistent: false);

                // On l'envoie vers la liste des étudiants
                return RedirectToAction("Index", "Students");
            }

            // Si la création a échoué (mot de passe trop faible, email déjà utilisé...),
            // Identity nous donne la liste des erreurs dans result.Errors.
            // On les ajoute une par une au ModelState pour qu'elles s'affichent dans le formulaire
            // (dans le bloc asp-validation-summary de la vue).
            // Le premier paramètre "string.Empty" veut dire : "cette erreur ne concerne pas un champ précis".
            foreach (IdentityError error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model); //On réaffiche le formulaire avec les erreurs
        }

        // ========================================================================================
        // CONNEXION - ÉTAPE 1 : afficher le formulaire de connexion
        // URL : GET /Account/Login
        //
        // returnUrl : quand un utilisateur NON connecté essaie d'ouvrir une page protégée (ex : /Students),
        // ASP.NET le redirige automatiquement ici en ajoutant l'adresse de la page demandée dans l'URL :
        //     /Account/Login?ReturnUrl=%2FStudents
        // On garde cette adresse pour pouvoir le renvoyer sur cette page une fois connecté.
        // Le "?" après string veut dire que returnUrl peut être null (si on arrive directement sur la page de connexion).
        // ========================================================================================
        public IActionResult Login(string? returnUrl)
        {
            // ViewData permet de transmettre une petite information à la vue (ici l'adresse de retour)
            ViewData["ReturnUrl"] = returnUrl;
            return View(); //Affiche Views/Account/Login.cshtml
        }

        // ========================================================================================
        // CONNEXION - ÉTAPE 2 : vérifier l'email et le mot de passe, puis CONNECTER l'utilisateur
        // URL : POST /Account/Login
        // ========================================================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl; //On le garde au cas où on doit réafficher le formulaire

            if (!ModelState.IsValid)
            {
                return View(model); //Champs vides ou email mal écrit : on réaffiche le formulaire avec les erreurs
            }

            // PasswordSignInAsync() fait tout le travail de connexion :
            //   1. cherche l'utilisateur dont le nom d'utilisateur est model.Email ;
            //   2. vérifie que le mot de passe est le bon ;
            //   3. si tout est bon, crée le cookie de connexion dans le navigateur.
            // Paramètres :
            //   - isPersistent: model.RememberMe -> reste connecté après fermeture du navigateur si la case est cochée ;
            //   - lockoutOnFailure: false -> on ne bloque pas le compte après plusieurs mauvais mots de passe (pour rester simple).
            Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.PasswordSignInAsync(
                model.Email, model.Password, isPersistent: model.RememberMe, lockoutOnFailure: false);
            // Remarque : on écrit le nom complet "Microsoft.AspNetCore.Identity.SignInResult" car il existe
            // une autre classe SignInResult dans ASP.NET MVC ; le nom complet évite toute confusion.

            if (result.Succeeded) //Email et mot de passe corrects
            {
                // Url.IsLocalUrl() vérifie que returnUrl est bien une adresse de NOTRE site.
                // C'est une sécurité importante : sans cette vérification, un pirate pourrait envoyer un lien du type
                // /Account/Login?ReturnUrl=https://site-pirate.com pour rediriger l'utilisateur vers un faux site après sa connexion.
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl); //On renvoie l'utilisateur sur la page qu'il voulait voir au départ
                }

                return RedirectToAction("Index", "Students"); //Sinon, on l'envoie vers la liste des étudiants
            }

            // Échec de la connexion.
            // On affiche volontairement un message GÉNÉRAL ("email ou mot de passe incorrect") sans préciser lequel des deux est faux :
            // cela évite qu'une personne mal intentionnée puisse savoir si un email possède un compte sur le site.
            ModelState.AddModelError(string.Empty, "Email ou mot de passe incorrect.");
            return View(model);
        }

        // ========================================================================================
        // DÉCONNEXION
        // URL : POST /Account/Logout
        // On utilise POST (bouton dans un formulaire) et pas un simple lien GET : sinon n'importe quel site
        // pourrait déconnecter l'utilisateur à son insu en lui faisant charger l'adresse /Account/Logout.
        // ========================================================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            // SignOutAsync() supprime le cookie de connexion : l'utilisateur n'est plus connecté.
            await _signInManager.SignOutAsync();

            return RedirectToAction("Login"); //On le renvoie vers la page de connexion
        }
    }
}
