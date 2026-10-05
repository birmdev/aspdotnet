using AppASPNETCore.Models.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore; //Pour pouvoir utiliser IdentityDbContext
using Microsoft.EntityFrameworkCore;

namespace AppASPNETCore.Data
{
    // ============================================================================================
    // StudentContext : l'UNIQUE contexte de l'application (fusion de StudentContext et AppASPNETCoreContext).
    // Un seul contexte = une seule base de données (StudentDb) qui contient à la fois :
    //   - la table Students (nos étudiants) ;
    //   - les tables d'Identity pour gérer les utilisateurs :
    //       * AspNetUsers : les comptes utilisateurs (email, mot de passe chiffré, ...)
    //       * AspNetRoles : les rôles (ex : "Admin"), pas utilisés pour l'instant
    //       * AspNetUserRoles, AspNetUserClaims, AspNetUserLogins, AspNetUserTokens, AspNetRoleClaims : tables techniques d'Identity
    //
    // Le contexte hérite de IdentityDbContext<ApplicationUser> :
    // - IdentityDbContext est lui-même un DbContext (on garde donc tout ce qu'on avait avant, dont la table Students),
    //   mais il ajoute automatiquement toutes les tables d'Identity listées ci-dessus.
    // - <ApplicationUser> indique la classe qui représente un utilisateur : c'est NOTRE classe ApplicationUser
    //   (Data/ApplicationUser.cs), qui hérite de IdentityUser. On pourra lui ajouter des propriétés plus tard
    //   (ex : Prénom, Nom) : elles deviendront de nouvelles colonnes de la table AspNetUsers.
    // ============================================================================================
    public class StudentContext : IdentityDbContext<ApplicationUser>
    {
        public StudentContext(DbContextOptions<StudentContext> options) : base(options)
        {
        }

        // OnModelCreating est appelée par Entity Framework au moment où il construit le "modèle"
        // (la description de toutes les tables, colonnes et relations de la base de données).
        // "override" : on remplace la méthode de la classe parente (IdentityDbContext) par la nôtre.
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // TRÈS IMPORTANT : on appelle d'abord la méthode de la classe parente.
            // C'est elle qui configure toutes les tables d'Identity (clés, index, longueurs des colonnes...).
            // Sans cette ligne, Entity Framework afficherait une erreur sur les tables AspNet...
            base.OnModelCreating(modelBuilder);

            // On peut ajouter ici des configurations supplémentaires si nécessaire, en utilisant le paramètre "modelBuilder".
            // Exemple (si un jour on ajoute une propriété FirstName à ApplicationUser) :
            //     modelBuilder.Entity<ApplicationUser>().Property(u => u.FirstName).HasMaxLength(50);
        }

        public DbSet<Student> Students { get; set; } //On dit à l'ORM qu'il va falloire utiliser la classe Student et que la classe Student est une entité qui sera mappée à une table dans la base de données. Le nom de la table sera automatiquement généré à partir du nom de la classe, mais on peut le personnaliser en utilisant l'attribut [Table("NomDeLaTable")] sur la classe Student.
    }
    //Le projet a maintenant un modèle de données (Student) et un contexte de données (StudentContext) qui permet d'interagir avec la base de données. Il est maintenant possible de créer un contrôleur pour gérer les opérations CRUD sur les étudiants. Pour ce faire, on doit créer un objet de Migration qui va permettre de créer la base de données et la table correspondante à l'entité Student. Pour cela, on utilise la commande suivante dans le terminal : dotnet ef migrations add InitialCreate. Cette commande va créer un dossier Migrations dans le projet avec un fichier InitialCreate.cs qui contient le code pour créer la table Students dans la base de données. Ensuite, on peut utiliser la commande dotnet ef database update pour appliquer cette migration et créer la base de données et la table correspondante.
}
