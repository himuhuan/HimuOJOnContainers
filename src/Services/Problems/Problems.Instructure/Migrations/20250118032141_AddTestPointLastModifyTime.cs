using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HimuOJ.Services.Problems.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTestPointLastModifyTime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifyTime",
                schema: "problems",
                table: "t_testpoints",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastModifyTime",
                schema: "problems",
                table: "t_testpoints");
        }
    }
}
