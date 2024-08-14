using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazorADAuth.Migrations
{
    /// <inheritdoc />
    public partial class AddFriendlyIdToAppRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FriendlyId",
                table: "AspNetRoles",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1000, 1");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_AspNetRoles_FriendlyId",
                table: "AspNetRoles",
                column: "FriendlyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "AK_AspNetRoles_FriendlyId",
                table: "AspNetRoles");

            migrationBuilder.DropColumn(
                name: "FriendlyId",
                table: "AspNetRoles");
        }
    }
}
