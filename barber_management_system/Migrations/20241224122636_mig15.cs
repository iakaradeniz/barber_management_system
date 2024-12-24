using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace barber_management_system.Migrations
{
    /// <inheritdoc />
    public partial class mig15 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OlusturmaTarihi",
                table: "Randevular",
                newName: "RandevuTarihi");

            migrationBuilder.AddColumn<int>(
                name: "Dakika",
                table: "Randevular",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "Ucret",
                table: "Randevular",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "CalisanUygunluklar",
                columns: table => new
                {
                    CalisanUygunlukID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CalisanID = table.Column<int>(type: "int", nullable: false),
                    Gun = table.Column<int>(type: "int", nullable: false),
                    BaslangicSaati = table.Column<TimeSpan>(type: "time", nullable: false),
                    BitisSaati = table.Column<TimeSpan>(type: "time", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CalisanUygunluklar", x => x.CalisanUygunlukID);
                    table.ForeignKey(
                        name: "FK_CalisanUygunluklar_Calisanlar_CalisanID",
                        column: x => x.CalisanID,
                        principalTable: "Calisanlar",
                        principalColumn: "CalisanID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CalisanUygunluklar_CalisanID",
                table: "CalisanUygunluklar",
                column: "CalisanID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CalisanUygunluklar");

            migrationBuilder.DropColumn(
                name: "Dakika",
                table: "Randevular");

            migrationBuilder.DropColumn(
                name: "Ucret",
                table: "Randevular");

            migrationBuilder.RenameColumn(
                name: "RandevuTarihi",
                table: "Randevular",
                newName: "OlusturmaTarihi");
        }
    }
}
