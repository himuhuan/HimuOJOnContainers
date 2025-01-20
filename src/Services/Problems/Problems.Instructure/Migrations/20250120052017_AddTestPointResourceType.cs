using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HimuOJ.Services.Problems.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTestPointResourceType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ResourceType",
                schema: "problems",
                table: "t_testpoints",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Text");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResourceType",
                schema: "problems",
                table: "t_testpoints");
        }
    }
}
