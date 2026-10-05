using AppASPNETCore.Data; //Pour pouvoir utiliser la classe StudentContext (notre lien avec la base de données)
using AppASPNETCore.Models.Entities; //Pour pouvoir utiliser la classe Student (notre modèle)
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
    // ============================================================================================
    public class StudentsController : Controller //On hérite de Controller pour avoir accès à View(), NotFound(), RedirectToAction(), ModelState...
    {
        // _context est la variable qui nous permet de lire et d'écrire dans la base de données.
        // "private" : elle n'est utilisable que dans cette classe.
        // "readonly" : elle ne peut être modifiée que dans le constructeur (on ne risque pas de l'écraser par erreur).
        // Par convention, on met un "_" devant le nom des variables privées d'une classe.
        private readonly StudentContext _context;

        // ----------------------------------------------------------------------------------------
        // CONSTRUCTEUR
        // On ne crée JAMAIS le contexte nous-mêmes avec "new StudentContext(...)".
        // C'est ASP.NET qui le crée et nous le donne automatiquement : c'est "l'injection de dépendances".
        // Cela fonctionne parce qu'on a déclaré le contexte dans Program.cs avec builder.Services.AddDbContext<StudentContext>(...).
        // ----------------------------------------------------------------------------------------
        public StudentsController(StudentContext context)
        {
            _context = context; //On garde le contexte reçu dans notre variable pour l'utiliser dans toutes les actions
        }

        // ========================================================================================
        // READ (Lire) : afficher la LISTE de tous les étudiants
        // URL : GET /Students  (ou /Students/Index)
        // ========================================================================================
        public IActionResult Index()
        {
            // _context.Students représente la table "Students" de la base de données.
            // ToList() va chercher toutes les lignes de la table et les transforme en une liste d'objets Student.
            // Entity Framework traduit cela tout seul en requête SQL : SELECT * FROM Students
            List<Student> students = _context.Students.ToList();

            // On envoie la liste à la vue Views/Students/Index.cshtml.
            // Dans la vue, cette liste sera accessible grâce à la variable "Model".
            return View(students);
        }

        // ========================================================================================
        // READ (Lire) : afficher le DÉTAIL d'un seul étudiant
        // URL : GET /Students/Details/{id}
        // Le paramètre "id" est rempli automatiquement avec la valeur présente dans l'URL.
        // ========================================================================================
        public IActionResult Details(Guid id)
        {
            // Find() cherche UN étudiant grâce à sa clé primaire (ici la propriété Id).
            // Requête SQL équivalente : SELECT * FROM Students WHERE Id = @id
            // Le "?" après Student veut dire que la variable peut valoir null (si aucun étudiant n'a cet Id).
            Student? student = _context.Students.Find(id);

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
        // Quand l'utilisateur clique sur "Ajouter un étudiant", le navigateur fait une requête GET :
        // on se contente d'afficher le formulaire.
        // ========================================================================================
        public IActionResult Create()
        {
            return View(); //Affiche Views/Students/Create.cshtml sans données (formulaire vide)
        }

        // ========================================================================================
        // CREATE (Créer) - ÉTAPE 2 : recevoir le formulaire rempli et ENREGISTRER l'étudiant
        // URL : POST /Students/Create
        // Quand l'utilisateur clique sur "Enregistrer", le navigateur envoie le formulaire avec une requête POST.
        // Les deux méthodes Create ont le même nom : c'est l'attribut [HttpPost] qui permet à ASP.NET
        // de savoir laquelle appeler (celle-ci uniquement pour les requêtes POST).
        // ========================================================================================
        [HttpPost] //Cette action ne répond qu'aux requêtes POST (envoi de formulaire)
        [ValidateAntiForgeryToken] //Sécurité : vérifie que le formulaire vient bien de NOTRE site (protection contre les attaques CSRF). Le jeton est ajouté automatiquement dans le formulaire par le tag <form asp-action="...">
        public IActionResult Create(Student student) //ASP.NET remplit automatiquement l'objet "student" avec les champs du formulaire (c'est le "model binding") : le champ "Name" va dans student.Name, etc.
        {
            // ModelState.IsValid vérifie que les données respectent les règles écrites dans la classe Student :
            // [Required] (champ obligatoire), [StringLength(100)] (100 caractères max), [EmailAddress] (format email valide)...
            if (!ModelState.IsValid) //Le "!" veut dire "NON" : donc "si les données ne sont PAS valides"
            {
                // On réaffiche le même formulaire, avec les valeurs déjà saisies par l'utilisateur
                // (pour qu'il n'ait pas à tout retaper) et les messages d'erreur à côté des champs.
                return View(student);
            }

            // Guid.NewGuid() génère un nouvel identifiant unique au hasard pour ce nouvel étudiant.
            student.Id = Guid.NewGuid();

            // Add() prépare l'ajout de l'étudiant... mais RIEN n'est encore écrit dans la base de données !
            _context.Students.Add(student);

            // SaveChanges() envoie VRAIMENT les modifications à la base de données.
            // Requête SQL équivalente : INSERT INTO Students (Id, Name, Email, Phone, Subscribed) VALUES (...)
            _context.SaveChanges();

            // Une fois l'enregistrement fait, on redirige l'utilisateur vers la liste des étudiants (action Index).
            // On redirige au lieu d'afficher directement une vue pour éviter que l'étudiant soit créé une 2ème fois
            // si l'utilisateur actualise la page (F5).
            return RedirectToAction("Index");
        }

        // ========================================================================================
        // UPDATE (Modifier) - ÉTAPE 1 : afficher le formulaire PRÉ-REMPLI avec les infos actuelles
        // URL : GET /Students/Edit/{id}
        // ========================================================================================
        public IActionResult Edit(Guid id)
        {
            // On cherche l'étudiant à modifier dans la base de données grâce à son Id
            Student? student = _context.Students.Find(id);

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
        public IActionResult Edit(Student student) //"student" contient les nouvelles valeurs du formulaire, ET son Id grâce au champ caché (input hidden) de la vue
        {
            // Même vérification que pour Create : si les données sont incorrectes,
            // on réaffiche le formulaire avec les messages d'erreur.
            if (!ModelState.IsValid)
            {
                return View(student);
            }

            // Update() indique à Entity Framework que cet étudiant a été modifié.
            // Il utilise l'Id de l'objet pour savoir quelle ligne de la table mettre à jour.
            _context.Students.Update(student);

            // On enregistre réellement les changements dans la base de données.
            // Requête SQL équivalente : UPDATE Students SET Name = ..., Email = ..., ... WHERE Id = ...
            _context.SaveChanges();

            return RedirectToAction("Index"); //Retour à la liste des étudiants
        }

        // ========================================================================================
        // DELETE (Supprimer) - ÉTAPE 1 : afficher une page de CONFIRMATION
        // URL : GET /Students/Delete/{id}
        // On ne supprime pas directement au clic : on demande d'abord confirmation à l'utilisateur
        // pour éviter les suppressions par erreur.
        // ========================================================================================
        public IActionResult Delete(Guid id)
        {
            // On cherche l'étudiant pour pouvoir afficher ses informations sur la page de confirmation
            Student? student = _context.Students.Find(id);

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
        public IActionResult DeleteConfirmed(Guid id) //L'id vient du champ caché (input hidden) du formulaire de la vue Delete
        {
            // On récupère l'étudiant à supprimer dans la base de données
            Student? student = _context.Students.Find(id);

            if (student != null) //On ne supprime que si l'étudiant existe bien ("!=" veut dire "différent de")
            {
                // Remove() prépare la suppression...
                _context.Students.Remove(student);

                // ...et SaveChanges() l'exécute réellement dans la base de données.
                // Requête SQL équivalente : DELETE FROM Students WHERE Id = ...
                _context.SaveChanges();
            }

            return RedirectToAction("Index"); //Retour à la liste des étudiants
        }
    }
}
