using System.ComponentModel.DataAnnotations; //Pour pouvoir utiliser les attributs de validation ([Required], [EmailAddress]...)

namespace AppASPNETCore.Models.ViewModels
{
    // ============================================================================================
    // LoginViewModel : représente les données du formulaire de CONNEXION.
    // Comme RegisterViewModel, c'est un "ViewModel" : il sert uniquement à transporter
    // les données du formulaire jusqu'au contrôleur (il n'est pas enregistré dans la base de données).
    // ============================================================================================
    public class LoginViewModel
    {
        [Required(ErrorMessage = "L'email est obligatoire")] //Le champ ne peut pas être vide
        [EmailAddress(ErrorMessage = "L'email n'est pas valide")] //Le texte doit avoir le format d'une adresse email
        [Display(Name = "Email")] //Texte affiché par défaut dans le <label> du formulaire
        public string Email { get; set; } = string.Empty; //"= string.Empty" : valeur de départ (texte vide) pour éviter une valeur null

        [Required(ErrorMessage = "Le mot de passe est obligatoire")]
        [DataType(DataType.Password)] //Le champ du formulaire sera de type "password" (caractères cachés)
        [Display(Name = "Mot de passe")]
        public string Password { get; set; } = string.Empty;

        // Case à cocher "Se souvenir de moi" :
        // - false (décochée) : l'utilisateur est déconnecté quand il ferme son navigateur.
        // - true (cochée)    : il reste connecté même après avoir fermé le navigateur (cookie persistant).
        [Display(Name = "Se souvenir de moi")]
        public bool RememberMe { get; set; }
    }
}
