using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace barber_management_system.Migrations
{
    /// <inheritdoc />
    public partial class mig7 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "HizmetSuresi",
                table: "Hizmetler",
                newName: "Dakika");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Dakika",
                table: "Hizmetler",
                newName: "HizmetSuresi");
        }
    }
}
