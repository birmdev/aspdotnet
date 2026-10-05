using System.ComponentModel.DataAnnotations; //Pour pouvoir utiliser les attributs de validation ([Required], [EmailAddress], [Compare]...)

namespace AppASPNETCore.Models.ViewModels
{
    // ============================================================================================
    // RegisterViewModel : représente les données du formulaire d'INSCRIPTION.
    //
    // Qu'est-ce qu'un "ViewModel" ?
    // C'est une classe qui sert UNIQUEMENT à transporter les données entre une vue (le formulaire)
    // et le contrôleur. Elle n'est PAS enregistrée telle quelle dans la base de données (ce n'est pas une entité).
    // Exemple : le champ "ConfirmPassword" n'a besoin d'exister que dans le formulaire, pour vérifier
    // que l'utilisateur n'a pas fait de faute de frappe. On ne l'enregistre jamais.
    //
    // Les attributs entre crochets [ ] sont des règles de validation : elles sont vérifiées
    // côté navigateur (jQuery Validation) ET côté serveur (ModelState.IsValid dans le contrôleur).
    // ============================================================================================
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "L'email est obligatoire")] //Le champ ne peut pas être vide
        [EmailAddress(ErrorMessage = "L'email n'est pas valide")] //Le texte doit avoir le format d'une adresse email
        [Display(Name = "Email")] //Texte affiché par défaut dans le <label> du formulaire
        public string Email { get; set; } = string.Empty; //"= string.Empty" : valeur de départ (texte vide) pour éviter une valeur null

        [Required(ErrorMessage = "Le mot de passe est obligatoire")]
        [DataType(DataType.Password)] //Indique que c'est un mot de passe : le champ du formulaire sera de type "password" (caractères cachés par des points)
        [Display(Name = "Mot de passe")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "La confirmation du mot de passe est obligatoire")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Les deux mots de passe ne correspondent pas")] //Vérifie que ce champ contient EXACTEMENT la même valeur que le champ Password
        [Display(Name = "Confirmer le mot de passe")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
