using AppASPNETCore.Data; //Pour pouvoir utiliser StudentContext (le lien avec la base de données)
using AppASPNETCore.Models.Entities; //Pour pouvoir utiliser la classe Student
using Microsoft.EntityFrameworkCore; //Pour pouvoir utiliser les méthodes asynchrones d'Entity Framework (ToListAsync, FindAsync, SaveChangesAsync...)

namespace AppASPNETCore.Services
{
    // ============================================================================================
    // StudentService : classe CONCRÈTE qui implémente (= respecte le contrat de) l'interface IStudentService.
    // Le ": IStudentService" après le nom de la classe veut dire "cette classe implémente IStudentService" :
    // elle est donc OBLIGÉE d'écrire le code de toutes les méthodes listées dans l'interface.
    //
    // Pourquoi un service ? (principes SOLID)
    // - "S" de SOLID = Single Responsibility (responsabilité unique) : chaque classe n'a qu'UN seul rôle.
    //   * Le CONTRÔLEUR s'occupe uniquement des requêtes HTTP (recevoir la demande, choisir la vue à afficher).
    //   * Le SERVICE s'occupe uniquement de la gestion des données des étudiants (lire, ajouter, modifier, supprimer).
    //   Avant, le contrôleur faisait les deux : maintenant, tout le code Entity Framework est regroupé ici.
    // - "O" de SOLID = Open/Closed (ouvert/fermé) : si on veut un autre comportement (ex : une autre base de données),
    //   on peut créer une NOUVELLE classe qui implémente IStudentService, au lieu de modifier le code existant.
    //
    // Ce service est enregistré dans Program.cs avec builder.Services.AddScoped<IStudentService, StudentService>();
    // C'est ce qui permet à ASP.NET de le fournir automatiquement au contrôleur (injection de dépendances).
    // ============================================================================================
    public class StudentService : IStudentService
    {
        // Le contexte Entity Framework : il nous permet de lire et d'écrire dans la base de données.
        private readonly StudentContext _context;

        // ----------------------------------------------------------------------------------------
        // CONSTRUCTEUR : le service a lui-même besoin du contexte.
        // Il le reçoit lui aussi par INJECTION DE DÉPENDANCES : on ne fait jamais "new StudentContext()",
        // c'est ASP.NET qui le crée et nous le donne (il est déclaré dans Program.cs avec AddDbContext).
        // ----------------------------------------------------------------------------------------
        public StudentService(StudentContext context)
        {
            _context = context;
        }

        // ----------------------------------------------------------------------------------------
        // Récupère TOUS les étudiants.
        // Le mot-clé "async" indique que la méthode contient des opérations asynchrones.
        // Le mot-clé "await" veut dire : "lance l'opération et attends son résultat SANS bloquer le serveur".
        // Pendant l'attente, le serveur peut s'occuper des requêtes d'autres utilisateurs.
        // ----------------------------------------------------------------------------------------
        public async Task<List<Student>> GetAllAsync()
        {
            // ToListAsync() = version asynchrone de ToList().
            // Requête SQL équivalente : SELECT * FROM Students
            return await _context.Students.ToListAsync();
        }

        // ----------------------------------------------------------------------------------------
        // Récupère UN étudiant grâce à son Id (clé primaire).
        // Renvoie null si aucun étudiant ne possède cet Id.
        // ----------------------------------------------------------------------------------------
        public async Task<Student?> GetByIdAsync(Guid id)
        {
            // FindAsync() = version asynchrone de Find().
            // Requête SQL équivalente : SELECT * FROM Students WHERE Id = @id
            return await _context.Students.FindAsync(id);
        }

        // ----------------------------------------------------------------------------------------
        // Ajoute un nouvel étudiant.
        // ----------------------------------------------------------------------------------------
        public async Task AddAsync(Student student)
        {
            // On génère un nouvel identifiant unique pour l'étudiant.
            // C'est le service (et non le contrôleur) qui s'en charge, car c'est une règle de gestion des données.
            student.Id = Guid.NewGuid();

            // Add() prépare l'ajout (rien n'est encore écrit dans la base de données).
            _context.Students.Add(student);

            // SaveChangesAsync() = version asynchrone de SaveChanges() : enregistre VRAIMENT dans la base.
            // Requête SQL équivalente : INSERT INTO Students (Id, Name, Email, Phone, Subscribed) VALUES (...)
            await _context.SaveChangesAsync();
        }

        // ----------------------------------------------------------------------------------------
        // Met à jour un étudiant existant avec les nouvelles valeurs reçues.
        // ----------------------------------------------------------------------------------------
        public async Task<bool> UpdateAsync(Student student)
        {
            // On va d'abord chercher dans la base l'étudiant tel qu'il est actuellement enregistré.
            Student? existingStudent = await _context.Students.FindAsync(student.Id);

            // S'il n'existe pas (par exemple s'il a été supprimé entre-temps par quelqu'un d'autre),
            // on renvoie false pour prévenir le contrôleur que la mise à jour n'a pas pu se faire.
            if (existingStudent == null)
            {
                return false;
            }

            // On recopie une par une les nouvelles valeurs dans l'étudiant existant.
            // Entity Framework "surveille" existingStudent : il détecte automatiquement ce qui a changé.
            existingStudent.Name = student.Name;
            existingStudent.Email = student.Email;
            existingStudent.Phone = student.Phone;
            existingStudent.Subscribed = student.Subscribed;

            // On enregistre les changements dans la base de données.
            // Requête SQL équivalente : UPDATE Students SET Name = ..., Email = ..., ... WHERE Id = ...
            await _context.SaveChangesAsync();
            return true; //La mise à jour a réussi
        }

        // ----------------------------------------------------------------------------------------
        // Supprime un étudiant grâce à son Id.
        // ----------------------------------------------------------------------------------------
        public async Task<bool> DeleteAsync(Guid id)
        {
            // On cherche l'étudiant à supprimer
            Student? student = await _context.Students.FindAsync(id);

            // S'il n'existe pas, il n'y a rien à supprimer : on renvoie false
            if (student == null)
            {
                return false;
            }

            // Remove() prépare la suppression...
            _context.Students.Remove(student);

            // ...et SaveChangesAsync() l'exécute réellement dans la base de données.
            // Requête SQL équivalente : DELETE FROM Students WHERE Id = ...
            await _context.SaveChangesAsync();
            return true; //La suppression a réussi
        }
    }
}
