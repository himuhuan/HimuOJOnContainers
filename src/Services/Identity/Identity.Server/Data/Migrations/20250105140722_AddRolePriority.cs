using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Identity.Server.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRolePriority : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Priority",
                table: "AspNetRoles",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Priority",
                table: "AspNetRoles");
        }
    }
}
