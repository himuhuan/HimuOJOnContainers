using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HimuOJ.Services.Submits.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveTaskIdAndFixMessageLength : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_t_submissions_TaskId",
                schema: "submits",
                table: "t_submissions");

            migrationBuilder.DropColumn(
                name: "TaskId",
                schema: "submits",
                table: "t_submissions");

            migrationBuilder.AlterColumn<string>(
                name: "StatusMessage",
                schema: "submits",
                table: "t_submissions",
                type: "character varying(10000)",
                maxLength: 10000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "StatusMessage",
                schema: "submits",
                table: "t_submissions",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(10000)",
                oldMaxLength: 10000,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TaskId",
                schema: "submits",
                table: "t_submissions",
                type: "character varying(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_t_submissions_TaskId",
                schema: "submits",
                table: "t_submissions",
                column: "TaskId");
        }
    }
}
