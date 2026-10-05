using System.ComponentModel.DataAnnotations;

namespace AppASPNETCore.Models.Entities
{
    public class Student
    {
        public Guid Id { get; set; } //GUID est un type de données qui représente un identifiant unique global (GUID) de 32 caractères (chiffres et lettres) qui ne se suivent pas, qui est utilisé pour identifier de manière unique un objet ou une entité dans un système informatique. C'est une première étape de sécurité. "Int" est limité à 2 milliards, "long" est limité à 9 milliards, "Guid" est illimité. Il est utilisé pour identifier de manière unique un objet ou une entité dans un système informatique.
        [Required] // Pour rendre Name obligatoire, on utilise l'attribut [Required] qui est une annotation de données (Data Annotation) qui permet de valider les données d'un modèle en indiquant qu'une propriété est obligatoire et ne peut pas être nulle ou vide. Cela permet de s'assurer que les données saisies par l'utilisateur respectent certaines contraintes avant d'être enregistrées dans la base de données.
        [StringLength(100)]
        public string Name { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        public string Phone { get; set; }
        public bool Subscribed { get; set; }
    }
}
