using Microsoft.AspNetCore.Identity;
namespace AppASPNETCore.Data;
// ApplicationUser : la classe qui représente un UTILISATEUR de l'application (une personne qui peut se connecter).
// Elle hérite de IdentityUser, elle possède donc déjà tout ce qu'il faut : Id, UserName, Email, PasswordHash (mot de passe chiffré)...
// Pour stocker des informations en plus sur l'utilisateur (ex : prénom, nom), il suffit d'ajouter des propriétés ici,
// puis de créer une migration (dotnet ef migrations add ...) pour ajouter les colonnes à la table AspNetUsers.
public class ApplicationUser : IdentityUser
{
}
