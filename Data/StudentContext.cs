using AppASPNETCore.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace AppASPNETCore.Data
{
    public class StudentContext : DbContext
    {
        public StudentContext(DbContextOptions<StudentContext> options) : base(options)
        {
        }

        public DbSet<Student> Students { get; set; } //On dit à l'ORM qu'il va falloire utiliser la classe Student et que la classe Student est une entité qui sera mappée à une table dans la base de données. Le nom de la table sera automatiquement généré à partir du nom de la classe, mais on peut le personnaliser en utilisant l'attribut [Table("NomDeLaTable")] sur la classe Student.
    }
    //Le projet a maintenant un modèle de données (Student) et un contexte de données (StudentContext) qui permet d'interagir avec la base de données. Il est maintenant possible de créer un contrôleur pour gérer les opérations CRUD sur les étudiants. Pour ce faire, on doit créer un objet de Migration qui va permettre de créer la base de données et la table correspondante à l'entité Student. Pour cela, on utilise la commande suivante dans le terminal : dotnet ef migrations add InitialCreate. Cette commande va créer un dossier Migrations dans le projet avec un fichier InitialCreate.cs qui contient le code pour créer la table Students dans la base de données. Ensuite, on peut utiliser la commande dotnet ef database update pour appliquer cette migration et créer la base de données et la table correspondante.
}
