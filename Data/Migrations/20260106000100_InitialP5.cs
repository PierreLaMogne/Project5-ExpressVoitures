using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Net_P5.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialP5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Marques",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nom = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Marques", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Modeles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nom = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MarqueId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Modeles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Modeles_Marques_MarqueId",
                        column: x => x.MarqueId,
                        principalTable: "Marques",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Finitions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nom = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModeleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Finitions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Finitions_Modeles_ModeleId",
                        column: x => x.ModeleId,
                        principalTable: "Modeles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Voitures",
                columns: table => new
                {
                    CodeVIN = table.Column<string>(type: "nvarchar(17)", maxLength: 17, nullable: false),
                    Annee = table.Column<int>(type: "int", nullable: false),
                    DateAchat = table.Column<DateOnly>(type: "date", nullable: false),
                    PrixAchat = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false),
                    EnVente = table.Column<bool>(type: "bit", nullable: false),
                    FinitionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Voitures", x => x.CodeVIN);
                    table.ForeignKey(
                        name: "FK_Voitures_Finitions_FinitionId",
                        column: x => x.FinitionId,
                        principalTable: "Finitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Reparations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Detail = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Cout = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false),
                    DateDisponibilite = table.Column<DateOnly>(type: "date", nullable: false),
                    VoitureCodeVIN = table.Column<string>(type: "nvarchar(17)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reparations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reparations_Voitures_VoitureCodeVIN",
                        column: x => x.VoitureCodeVIN,
                        principalTable: "Voitures",
                        principalColumn: "CodeVIN",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Ventes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateVente = table.Column<DateOnly>(type: "date", nullable: false),
                    VoitureCodeVIN = table.Column<string>(type: "nvarchar(17)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ventes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ventes_Voitures_VoitureCodeVIN",
                        column: x => x.VoitureCodeVIN,
                        principalTable: "Voitures",
                        principalColumn: "CodeVIN",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Finitions_ModeleId",
                table: "Finitions",
                column: "ModeleId");

            migrationBuilder.CreateIndex(
                name: "IX_Modeles_MarqueId",
                table: "Modeles",
                column: "MarqueId");

            migrationBuilder.CreateIndex(
                name: "IX_Reparations_VoitureCodeVIN",
                table: "Reparations",
                column: "VoitureCodeVIN");

            migrationBuilder.CreateIndex(
                name: "IX_Ventes_VoitureCodeVIN",
                table: "Ventes",
                column: "VoitureCodeVIN");

            migrationBuilder.CreateIndex(
                name: "IX_Voitures_FinitionId",
                table: "Voitures",
                column: "FinitionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Reparations");

            migrationBuilder.DropTable(
                name: "Ventes");

            migrationBuilder.DropTable(
                name: "Voitures");

            migrationBuilder.DropTable(
                name: "Finitions");

            migrationBuilder.DropTable(
                name: "Modeles");

            migrationBuilder.DropTable(
                name: "Marques");
        }
    }
}
