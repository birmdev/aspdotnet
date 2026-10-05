using AppASPNETCore.Models.Entities; //Pour pouvoir utiliser la classe Student (notre modèle)
using AppASPNETCore.Services; //Pour pouvoir utiliser l'interface IStudentService (notre service)
using Microsoft.AspNetCore.Authorization; //Pour pouvoir utiliser l'attribut [Authorize]
using Microsoft.AspNetCore.Mvc; //Pour pouvoir utiliser Controller, IActionResult, [HttpPost], etc.

namespace AppASPNETCore.Controllers
{
    // ============================================================================================
    // StudentsController : c'est le contrôleur qui gère le CRUD des étudiants.
    // CRUD = Create (créer), Read (lire), Update (modifier), Delete (supprimer).
    //
    // Le nom de la classe se termine OBLIGATOIREMENT par "Controller" : c'est une convention d'ASP.NET.
    // Grâce à cette convention, l'URL "/Students" sera automatiquement dirigée vers ce contrôleur.
    // Exemples d'URL :
    //   /Students            -> méthode Index()   (liste des étudiants)
    //   /Students/Create     -> méthode Create()  (formulaire d'ajout)
    //   /Students/Edit/{id}  -> méthode Edit(id)  (formulaire de modification)
    //
    // Chaque méthode publique est appelée une "action". Une action renvoie le plus souvent une Vue
    // (un fichier .cshtml du dossier Views/Students/ qui porte le même nom que l'action).
    //
    // PRINCIPES SOLID :
    // Ce contrôleur ne parle PLUS directement à la base de données (il n'utilise plus StudentContext).
    // Il passe par le service IStudentService, qui s'occupe de toute la gestion des données.
    // - "S" (responsabilité unique) : le contrôleur gère uniquement les requêtes HTTP et le choix des vues.
    // - "D" (inversion des dépendances) : le contrôleur dépend de l'INTERFACE IStudentService,
    //   et pas de la classe concrète StudentService.
    //
    // ASYNCHRONE :
    // Toutes les actions qui accèdent aux données sont asynchrones ("async Task<IActionResult>").
    // Pendant qu'une action attend la base de données, le serveur reste libre de répondre
    // aux autres utilisateurs : il n'est pas bloqué si plusieurs personnes utilisent le site en même temps.
    //
    // AUTHENTIFICATION :
    // L'attribut [Authorize] placé au-dessus de la classe protège TOUTES les actions de ce contrôleur :
    // seuls les utilisateurs CONNECTÉS peuvent voir, ajouter, modifier ou supprimer des étudiants.
    // Si une personne non connectée essaie d'ouvrir une de ces pages, elle est automatiquement
    // redirigée vers la page de connexion (/Account/Login, réglée dans Program.cs).
    // ============================================================================================
    [Authorize]
    public class StudentsController : Controller //On hérite de Controller pour avoir accès à View(), NotFound(), RedirectToAction(), ModelState...
    {
        // _studentService est le service qui gère les étudiants.
        // Son type est l'INTERFACE IStudentService : le contrôleur ne sait pas quelle classe se cache derrière.
        // "private" : utilisable uniquement dans cette classe.
        // "readonly" : ne peut être modifiée que dans le constructeur (on ne risque pas de l'écraser par erreur).
        // Par convention, on met un "_" devant le nom des variables privées d'une classe.
        private readonly IStudentService _studentService;

        // ----------------------------------------------------------------------------------------
        // CONSTRUCTEUR : INJECTION DE DÉPENDANCES
        // On ne crée JAMAIS le service nous-mêmes avec "new StudentService(...)".
        // Le contrôleur DEMANDE un IStudentService dans son constructeur, et ASP.NET le lui DONNE automatiquement.
        // ASP.NET sait quelle classe utiliser grâce à la ligne de Program.cs :
        //     builder.Services.AddScoped<IStudentService, StudentService>();
        // qui veut dire : "quand quelqu'un demande un IStudentService, donne-lui un StudentService".
        // ----------------------------------------------------------------------------------------
        public StudentsController(IStudentService studentService)
        {
            _studentService = studentService; //On garde le service reçu pour l'utiliser dans toutes les actions
        }

        // ========================================================================================
        // READ (Lire) : afficher la LISTE de tous les étudiants
        // URL : GET /Students  (ou /Students/Index)
        //
        // "async" : la méthode contient des opérations asynchrones (avec "await").
        // "Task<IActionResult>" : la méthode renverra un IActionResult (une vue, une redirection...)
        // une fois que l'opération asynchrone sera terminée.
        // ========================================================================================
        public async Task<IActionResult> Index()
        {
            // On demande au service la liste de tous les étudiants.
            // "await" : on attend la réponse de la base de données SANS bloquer le serveur.
            List<Student> students = await _studentService.GetAllAsync();

            // On envoie la liste à la vue Views/Students/Index.cshtml.
            // Dans la vue, cette liste sera accessible grâce à la variable "Model".
            return View(students);
        }

        // ========================================================================================
        // READ (Lire) : afficher le DÉTAIL d'un seul étudiant
        // URL : GET /Students/Details/{id}
        // Le paramètre "id" est rempli automatiquement avec la valeur présente dans l'URL.
        // ========================================================================================
        public async Task<IActionResult> Details(Guid id)
        {
            // On demande au service l'étudiant qui possède cet Id.
            // Le "?" après Student veut dire que la variable peut valoir null (si aucun étudiant n'a cet Id).
            Student? student = await _studentService.GetByIdAsync(id);

            // Si aucun étudiant n'a été trouvé (par exemple si quelqu'un tape un faux Id dans l'URL)...
            if (student == null)
            {
                return NotFound(); //...on renvoie une erreur 404 "Page non trouvée"
            }

            // Sinon, on envoie l'étudiant trouvé à la vue Views/Students/Details.cshtml
            return View(student);
        }

        // ========================================================================================
        // CREATE (Créer) - ÉTAPE 1 : afficher le formulaire VIDE
        // URL : GET /Students/Create
        // Cette action n'accède pas à la base de données : elle n'a donc pas besoin d'être asynchrone.
        // ========================================================================================
        public IActionResult Create()
        {
            return View(); //Affiche Views/Students/Create.cshtml sans données (formulaire vide)
        }

        // ========================================================================================
        // CREATE (Créer) - ÉTAPE 2 : recevoir le formulaire rempli et ENREGISTRER l'étudiant
        // URL : POST /Students/Create
        // Les deux méthodes Create ont le même nom : c'est l'attribut [HttpPost] qui permet à ASP.NET
        // de savoir laquelle appeler (celle-ci uniquement pour les requêtes POST).
        // ========================================================================================
        [HttpPost] //Cette action ne répond qu'aux requêtes POST (envoi de formulaire)
        [ValidateAntiForgeryToken] //Sécurité : vérifie que le formulaire vient bien de NOTRE site (protection contre les attaques CSRF). Le jeton est ajouté automatiquement dans le formulaire par le tag <form asp-action="...">
        public async Task<IActionResult> Create(Student student) //ASP.NET remplit automatiquement l'objet "student" avec les champs du formulaire (c'est le "model binding") : le champ "Name" va dans student.Name, etc.
        {
            // ModelState.IsValid vérifie que les données respectent les règles écrites dans la classe Student :
            // [Required] (champ obligatoire), [StringLength(100)] (100 caractères max), [EmailAddress] (format email valide)...
            if (!ModelState.IsValid) //Le "!" veut dire "NON" : donc "si les données ne sont PAS valides"
            {
                // On réaffiche le même formulaire, avec les valeurs déjà saisies par l'utilisateur
                // (pour qu'il n'ait pas à tout retaper) et les messages d'erreur à côté des champs.
                return View(student);
            }

            // On demande au service d'ajouter l'étudiant (c'est lui qui génère l'Id et enregistre en base).
            await _studentService.AddAsync(student);

            // Une fois l'enregistrement fait, on redirige l'utilisateur vers la liste des étudiants (action Index).
            // On redirige au lieu d'afficher directement une vue pour éviter que l'étudiant soit créé une 2ème fois
            // si l'utilisateur actualise la page (F5).
            return RedirectToAction("Index");
        }

        // ========================================================================================
        // UPDATE (Modifier) - ÉTAPE 1 : afficher le formulaire PRÉ-REMPLI avec les infos actuelles
        // URL : GET /Students/Edit/{id}
        // ========================================================================================
        public async Task<IActionResult> Edit(Guid id)
        {
            // On demande au service l'étudiant à modifier
            Student? student = await _studentService.GetByIdAsync(id);

            if (student == null) //Aucun étudiant avec cet Id
            {
                return NotFound(); //Erreur 404
            }

            // On envoie l'étudiant à la vue Views/Students/Edit.cshtml : les champs du formulaire
            // seront automatiquement pré-remplis avec ses informations actuelles.
            return View(student);
        }

        // ========================================================================================
        // UPDATE (Modifier) - ÉTAPE 2 : recevoir le formulaire modifié et METTRE À JOUR l'étudiant
        // URL : POST /Students/Edit
        // ========================================================================================
        [HttpPost] //Uniquement pour les requêtes POST (envoi du formulaire)
        [ValidateAntiForgeryToken] //Sécurité anti-CSRF (voir l'explication dans Create)
        public async Task<IActionResult> Edit(Student student) //"student" contient les nouvelles valeurs du formulaire, ET son Id grâce au champ caché (input hidden) de la vue
        {
            // Même vérification que pour Create : si les données sont incorrectes,
            // on réaffiche le formulaire avec les messages d'erreur.
            if (!ModelState.IsValid)
            {
                return View(student);
            }

            // On demande au service de mettre à jour l'étudiant.
            // Le service renvoie true si tout s'est bien passé, false si l'étudiant n'existe pas.
            bool updated = await _studentService.UpdateAsync(student);

            if (!updated) //L'étudiant n'existe pas (ou a été supprimé entre-temps)
            {
                return NotFound(); //Erreur 404
            }

            return RedirectToAction("Index"); //Retour à la liste des étudiants
        }

        // ========================================================================================
        // DELETE (Supprimer) - ÉTAPE 1 : afficher une page de CONFIRMATION
        // URL : GET /Students/Delete/{id}
        // On ne supprime pas directement au clic : on demande d'abord confirmation à l'utilisateur
        // pour éviter les suppressions par erreur.
        // ========================================================================================
        public async Task<IActionResult> Delete(Guid id)
        {
            // On récupère l'étudiant pour pouvoir afficher ses informations sur la page de confirmation
            Student? student = await _studentService.GetByIdAsync(id);

            if (student == null) //Aucun étudiant avec cet Id
            {
                return NotFound(); //Erreur 404
            }

            return View(student); //Affiche Views/Students/Delete.cshtml ("Voulez-vous vraiment supprimer... ?")
        }

        // ========================================================================================
        // DELETE (Supprimer) - ÉTAPE 2 : SUPPRIMER réellement l'étudiant après confirmation
        // URL : POST /Students/DeleteConfirmed
        // Cette méthode s'appelle DeleteConfirmed et pas Delete, car en C# on ne peut pas avoir deux
        // méthodes avec le même nom ET les mêmes paramètres (ici les deux recevraient un Guid id).
        // ========================================================================================
        [HttpPost] //Uniquement pour les requêtes POST (clic sur le bouton "Supprimer" du formulaire)
        [ValidateAntiForgeryToken] //Sécurité anti-CSRF (voir l'explication dans Create)
        public async Task<IActionResult> DeleteConfirmed(Guid id) //L'id vient du champ caché (input hidden) du formulaire de la vue Delete
        {
            // On demande au service de supprimer l'étudiant.
            // Si l'étudiant n'existe déjà plus, le service renvoie simplement false :
            // dans ce cas il n'y a rien à faire, on retourne quand même à la liste.
            await _studentService.DeleteAsync(id);

            return RedirectToAction("Index"); //Retour à la liste des étudiants
        }
    }
}
