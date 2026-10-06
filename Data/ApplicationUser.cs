using System.ComponentModel.DataAnnotations; //Pour pouvoir utiliser l'attribut [StringLength]
using Microsoft.AspNetCore.Identity;
namespace AppASPNETCore.Data;
// ApplicationUser : la classe qui représente un UTILISATEUR de l'application (une personne qui peut se connecter).
// Elle hérite de IdentityUser, elle possède donc déjà tout ce qu'il faut : Id, UserName, Email, PasswordHash (mot de passe chiffré)...
// Pour stocker des informations en plus sur l'utilisateur, il suffit d'ajouter des propriétés ici,
// puis de créer une migration (dotnet ef migrations add ...) pour ajouter les colonnes à la table AspNetUsers.
public class ApplicationUser : IdentityUser
{
    // Prénom de l'utilisateur : devient la colonne "FirstName" de la table AspNetUsers (migration AddUserNames).
    // [StringLength(50)] : 50 caractères maximum (la colonne sera de type nvarchar(50) dans la base de données).
    // [PersonalData] : indique à Identity que c'est une donnée personnelle. Elle sera donc incluse dans le fichier
    // téléchargé depuis la page "Gérer mon compte > Personal data" (obligation du RGPD).
    // "= string.Empty" : valeur par défaut (texte vide), pour éviter une valeur null.
    [StringLength(50)]
    [PersonalData]
    public string FirstName { get; set; } = string.Empty;

    // Nom de famille de l'utilisateur : devient la colonne "LastName" de la table AspNetUsers.
    [StringLength(50)]
    [PersonalData]
    public string LastName { get; set; } = string.Empty;
}
