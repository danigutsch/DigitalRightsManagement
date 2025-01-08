using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DigitalRightsManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeCreatedByToManagerInProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "Products",
                newName: "Manager");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Manager",
                table: "Products",
                newName: "CreatedBy");
        }
    }
}
