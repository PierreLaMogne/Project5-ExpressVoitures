using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Net_P5.Migrations
{
    /// <inheritdoc />
    public partial class FinalP5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Voitures_Marques_MarqueId",
                table: "Voitures");

            migrationBuilder.DropForeignKey(
                name: "FK_Voitures_Modeles_ModeleId",
                table: "Voitures");

            migrationBuilder.DropIndex(
                name: "IX_Voitures_MarqueId",
                table: "Voitures");

            migrationBuilder.DropIndex(
                name: "IX_Voitures_ModeleId",
                table: "Voitures");

            migrationBuilder.DropColumn(
                name: "MarqueId",
                table: "Voitures");

            migrationBuilder.DropColumn(
                name: "ModeleId",
                table: "Voitures");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MarqueId",
                table: "Voitures",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ModeleId",
                table: "Voitures",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Voitures_MarqueId",
                table: "Voitures",
                column: "MarqueId");

            migrationBuilder.CreateIndex(
                name: "IX_Voitures_ModeleId",
                table: "Voitures",
                column: "ModeleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Voitures_Marques_MarqueId",
                table: "Voitures",
                column: "MarqueId",
                principalTable: "Marques",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Voitures_Modeles_ModeleId",
                table: "Voitures",
                column: "ModeleId",
                principalTable: "Modeles",
                principalColumn: "Id");
        }
    }
}
