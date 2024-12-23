using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace barber_management_system.Migrations
{
    /// <inheritdoc />
    public partial class mig6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "HizmetSuresi",
                table: "Hizmetler",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HizmetSuresi",
                table: "Hizmetler");
        }
    }
}
