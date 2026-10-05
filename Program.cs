using AppASPNETCore.Data;
using AppASPNETCore.Services; //Pour pouvoir utiliser IStudentService et StudentService
using Microsoft.AspNetCore.Identity; //Pour pouvoir utiliser IdentityRole (gestion des utilisateurs)
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

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
// AUTHENTIFICATION avec ASP.NET Core Identity
// AddIdentity enregistre tous les outils d'Identity pour l'injection de dépendances :
// UserManager (gérer les comptes) et SignInManager (connexion/déconnexion), utilisés dans AccountController.
// - ApplicationUser : NOTRE classe qui représente un utilisateur (Data/ApplicationUser.cs, elle hérite de IdentityUser).
// - IdentityRole : la classe qui représente un rôle (ex : "Admin"), pas utilisée pour l'instant.
// ATTENTION : Identity ne doit être enregistré qu'UNE SEULE FOIS (avec AddIdentity OU AddDefaultIdentity, jamais les deux).
// Sinon, l'application plante au démarrage avec l'erreur "Scheme already exists: Identity.Application".
// ================================================================================================
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
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
    // On indique à Identity qu'il doit enregistrer les utilisateurs dans NOTRE base de données,
    // grâce à notre StudentContext (le seul contexte de l'application, qui hérite de IdentityDbContext<ApplicationUser>).
    .AddEntityFrameworkStores<StudentContext>()
    // Ajoute les outils qui génèrent des "jetons" (tokens), utilisés par exemple pour réinitialiser un mot de passe.
    .AddDefaultTokenProviders();

// Réglages du COOKIE de connexion.
// Après une connexion réussie, Identity dépose un cookie dans le navigateur : c'est lui qui prouve,
// à chaque nouvelle page, que l'utilisateur est bien connecté.
builder.Services.ConfigureApplicationCookie(options =>
{
    // Page vers laquelle un utilisateur NON connecté est redirigé s'il essaie d'ouvrir une page protégée par [Authorize]
    options.LoginPath = "/Account/Login";
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


app.Run();