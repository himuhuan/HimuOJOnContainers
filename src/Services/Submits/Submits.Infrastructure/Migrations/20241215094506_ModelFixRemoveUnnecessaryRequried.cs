using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HimuOJ.Services.Submits.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ModelFixRemoveUnnecessaryRequried : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "UsedTimeMs",
                schema: "submits",
                table: "t_testpoint_results",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<long>(
                name: "UsedMemoryByte",
                schema: "submits",
                table: "t_testpoint_results",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<int>(
                name: "Difference_Position",
                schema: "submits",
                table: "t_testpoint_results",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "UsedTimeMs",
                schema: "submits",
                table: "t_testpoint_results",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "UsedMemoryByte",
                schema: "submits",
                table: "t_testpoint_results",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Difference_Position",
                schema: "submits",
                table: "t_testpoint_results",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);
        }
    }
}
