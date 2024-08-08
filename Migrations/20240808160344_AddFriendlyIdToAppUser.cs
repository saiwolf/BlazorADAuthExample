using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazorADAuth.Migrations
{
    /// <inheritdoc />
    public partial class AddFriendlyIdToAppUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FriendlyId",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1000, 1");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_AspNetUsers_FriendlyId",
                table: "AspNetUsers",
                column: "FriendlyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "AK_AspNetUsers_FriendlyId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "FriendlyId",
                table: "AspNetUsers");
        }
    }
}
