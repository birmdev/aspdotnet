using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppASPNETCore.Migrations
{
    // Migration AddUserNames : ajoute les colonnes FirstName (prénom) et LastName (nom) à la table AspNetUsers,
    // suite à l'ajout des propriétés FirstName et LastName dans la classe ApplicationUser.
    // defaultValue: "" : les comptes qui existent déjà reçoivent un prénom et un nom vides (ils pourront les remplir
    // depuis la page "Gérer mon compte").
    /// <inheritdoc />
    public partial class AddUserNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "AspNetUsers",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "AspNetUsers",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "AspNetUsers");
        }
    }
}
