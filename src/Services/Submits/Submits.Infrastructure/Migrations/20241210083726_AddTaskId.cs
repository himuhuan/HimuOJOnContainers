using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HimuOJ.Services.Submits.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTaskId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_t_submissions_TaskId",
                schema: "submits",
                table: "t_submissions");

            migrationBuilder.DropColumn(
                name: "TaskId",
                schema: "submits",
                table: "t_submissions");
        }
    }
}
