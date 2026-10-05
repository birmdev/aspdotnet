using AppASPNETCore.Data;
using AppASPNETCore.Services; //Pour pouvoir utiliser IStudentService et StudentService
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

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();