using AppASPNETCore.Data;
using AppASPNETCore.Services; //Pour pouvoir utiliser IStudentService et StudentService
using Microsoft.AspNetCore.Identity; //Pour pouvoir utiliser IdentityRole (gestion des utilisateurs)
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// AddRazorPages : active les "Razor Pages", un autre type de pages ASP.NET (un fichier .cshtml + un fichier .cshtml.cs).
// On en a besoin car les pages d'Identity (connexion, inscription, déconnexion...) du dossier Areas/Identity/Pages
// sont des Razor Pages, et pas des vues MVC avec un contrôleur.
builder.Services.AddRazorPages();

builder.Services.AddDbContext<StudentContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("StudentContext")));

// Enregistrement du service des étudiants pour l'INJECTION DE DÉPENDANCES.
// Cette ligne veut dire : "quand une classe demande un IStudentService dans son constructeur,
// crée et donne-lui un objet StudentService".
// AddScoped : un nouvel objet StudentService est créé pour CHAQUE requête HTTP, puis réutilisé
// pendant toute cette requête (c'est la même durée de vie que le StudentContext, ce qui est conseillé).
// Si un jour on veut changer de service, il suffira de modifier cette ligne : le contrôleur ne changera pas.
builder.Services.AddScoped<IStudentService, StudentService>();

// ================================================================================================
// AUTHENTIFICATION avec ASP.NET Core Identity + Identity UI (package Microsoft.AspNetCore.Identity.UI)
// AddDefaultIdentity enregistre en une seule ligne :
//   - tous les outils d'Identity pour l'injection de dépendances : UserManager (gérer les comptes)
//     et SignInManager (connexion/déconnexion), utilisés par les pages du dossier Areas/Identity/Pages ;
//   - l'interface utilisateur d'Identity (Identity UI) : les pages toutes prêtes de connexion, d'inscription,
//     de déconnexion, de gestion du compte... accessibles aux adresses /Identity/Account/...
//   - le cookie de connexion.
// <ApplicationUser> : NOTRE classe qui représente un utilisateur (Data/ApplicationUser.cs, elle hérite de IdentityUser).
// ATTENTION : Identity ne doit être enregistré qu'UNE SEULE FOIS (avec AddIdentity OU AddDefaultIdentity, jamais les deux).
// Sinon, l'application plante au démarrage avec l'erreur "Scheme already exists: Identity.Application".
// ================================================================================================
builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    // RequireConfirmedAccount = false : l'utilisateur peut se connecter juste après son inscription.
    // Si on mettait true, il devrait d'abord cliquer sur un lien reçu par email pour confirmer son compte.
    // Or notre application ne sait pas (encore) envoyer d'emails : personne ne pourrait donc jamais se connecter.
    options.SignIn.RequireConfirmedAccount = false;

    // MaxLengthForKeys = 0 : pas de longueur maximale imposée pour les colonnes "clés" des tables d'Identity.
    // AddDefaultIdentity limite sinon ces colonnes à 128 caractères, alors que notre base de données les a déjà créées
    // avec 450 caractères (migration AddIdentity). Sans cette ligne, il faudrait une nouvelle migration pour modifier
    // ces colonnes, et SQL Server refuse de modifier une colonne qui fait partie d'une clé primaire.
    options.Stores.MaxLengthForKeys = 0;

    // Règles que doit respecter un mot de passe lors de l'inscription.
    // (Ce sont les règles par défaut d'Identity, écrites ici pour qu'on les voie clairement.)
    options.Password.RequiredLength = 6;              //6 caractères minimum
    options.Password.RequireDigit = true;             //au moins un chiffre (0-9)
    options.Password.RequireLowercase = true;         //au moins une lettre minuscule (a-z)
    options.Password.RequireUppercase = true;         //au moins une lettre majuscule (A-Z)
    options.Password.RequireNonAlphanumeric = true;   //au moins un caractère spécial (! @ # $ % ...)

    // Deux comptes ne peuvent pas utiliser la même adresse email
    options.User.RequireUniqueEmail = true;
})
    // AddRoles : active la gestion des rôles (ex : "Admin"), avec la classe IdentityRole. Pas utilisée pour l'instant,
    // mais cela permettra plus tard de réserver certaines pages à certains utilisateurs.
    .AddRoles<IdentityRole>()
    // On indique à Identity qu'il doit enregistrer les utilisateurs dans NOTRE base de données,
    // grâce à notre StudentContext (le seul contexte de l'application, qui hérite de IdentityDbContext<ApplicationUser>).
    .AddEntityFrameworkStores<StudentContext>();

// Réglages du COOKIE de connexion.
// Après une connexion réussie, Identity dépose un cookie dans le navigateur : c'est lui qui prouve,
// à chaque nouvelle page, que l'utilisateur est bien connecté.
builder.Services.ConfigureApplicationCookie(options =>
{
    // Page vers laquelle un utilisateur NON connecté est redirigé s'il essaie d'ouvrir une page protégée par [Authorize].
    // C'est la page de connexion fournie par Identity UI (fichier Areas/Identity/Pages/Account/Login.cshtml).
    // ("/Identity" est le nom de l'"Area", c'est-à-dire du dossier Areas/Identity.)
    options.LoginPath = "/Identity/Account/Login";
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

// UseAuthentication : à chaque requête, lit le cookie de connexion pour savoir QUI est l'utilisateur
// (ou s'il n'est pas connecté). Cette ligne doit OBLIGATOIREMENT être placée AVANT UseAuthorization.
app.UseAuthentication();

// UseAuthorization : vérifie si l'utilisateur a le DROIT d'accéder à la page demandée
// (par exemple : les pages marquées [Authorize] exigent d'être connecté).
// L'ordre est logique : on doit d'abord savoir QUI est l'utilisateur, avant de vérifier ce qu'il a le droit de faire.
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

// MapRazorPages : crée les adresses (routes) des Razor Pages.
// Sans cette ligne, les pages d'Identity (/Identity/Account/Login, /Identity/Account/Register...) renverraient une erreur 404.
app.MapRazorPages()
    .WithStaticAssets();

app.Run();