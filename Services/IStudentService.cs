using AppASPNETCore.Models.Entities; //Pour pouvoir utiliser la classe Student

namespace AppASPNETCore.Services
{
    // ============================================================================================
    // IStudentService : INTERFACE du service des étudiants.
    //
    // Une interface est un "contrat" : elle liste les méthodes qui DOIVENT exister,
    // mais elle ne contient AUCUN code (pas de corps de méthode, juste leur signature).
    // Par convention, le nom d'une interface commence par un "I" majuscule.
    //
    // Pourquoi une interface ? (principes SOLID)
    // - "D" de SOLID = Dependency Inversion (inversion des dépendances) :
    //   le contrôleur dépend de cette interface (une abstraction) et PAS de la classe concrète StudentService.
    //   Le contrôleur sait donc CE QUE le service sait faire, sans savoir COMMENT il le fait.
    //   On pourrait remplacer StudentService par une autre classe (ex : un faux service pour les tests,
    //   ou un service qui lit un fichier au lieu d'une base de données) SANS toucher au contrôleur.
    // - "I" de SOLID = Interface Segregation (ségrégation des interfaces) :
    //   l'interface ne contient que les méthodes utiles pour gérer les étudiants, rien de plus.
    //
    // Pourquoi des méthodes asynchrones (Task, Async) ?
    // Une requête vers la base de données prend du temps (attente du disque, du réseau...).
    // En asynchrone, pendant cette attente, le serveur n'est PAS bloqué : il peut traiter
    // les requêtes des autres utilisateurs en même temps. Quand la base répond, le code reprend.
    // - "Task" (sans <>) = une opération asynchrone qui ne renvoie rien (équivalent de "void").
    // - "Task<X>"        = une opération asynchrone qui renverra une valeur de type X une fois terminée.
    // - Par convention, le nom d'une méthode asynchrone se termine par "Async".
    // ============================================================================================
    public interface IStudentService
    {
        // Renvoie la liste de TOUS les étudiants
        Task<List<Student>> GetAllAsync();

        // Renvoie UN étudiant grâce à son Id, ou null s'il n'existe pas (d'où le "?" après Student)
        Task<Student?> GetByIdAsync(Guid id);

        // Ajoute un nouvel étudiant dans la base de données
        Task AddAsync(Student student);

        // Met à jour un étudiant existant.
        // Renvoie true si la mise à jour a réussi, false si l'étudiant n'existe pas (ou plus).
        Task<bool> UpdateAsync(Student student);

        // Supprime un étudiant grâce à son Id.
        // Renvoie true si la suppression a réussi, false si l'étudiant n'existe pas (ou plus).
        Task<bool> DeleteAsync(Guid id);
    }
}
