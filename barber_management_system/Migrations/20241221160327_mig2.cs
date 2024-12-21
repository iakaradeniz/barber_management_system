using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace barber_management_system.Migrations
{
    /// <inheritdoc />
    public partial class mig2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "99398e26-c499-4ad4-b4da-aab4b16096b1");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c8c657ef-588f-48fb-91a4-a5ec62717ff2");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "fdad3bb2-24b5-4804-ab29-8bd96140276c");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "74353541-a227-43da-8cf4-67bf93c2ce36", null, "Admin", "ADMIN" },
                    { "8c56f3a5-afad-4911-b252-83002871d2b6", null, "Calisan", "CALISAN" },
                    { "eb3e0b7d-2ce6-43e5-8d15-eeff39ad353c", null, "Musteri", "MUSTERI" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "74353541-a227-43da-8cf4-67bf93c2ce36");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "8c56f3a5-afad-4911-b252-83002871d2b6");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "eb3e0b7d-2ce6-43e5-8d15-eeff39ad353c");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "99398e26-c499-4ad4-b4da-aab4b16096b1", null, "Admin", "ADMIN" },
                    { "c8c657ef-588f-48fb-91a4-a5ec62717ff2", null, "Musteri", "MUSTERI" },
                    { "fdad3bb2-24b5-4804-ab29-8bd96140276c", null, "Calisan", "CALISAN" }
                });
        }
    }
}
