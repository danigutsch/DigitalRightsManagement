using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DigitalRightsManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeNameOfProductOwnerColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Manager",
                table: "Products",
                newName: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Products",
                newName: "Manager");
        }
    }
}
