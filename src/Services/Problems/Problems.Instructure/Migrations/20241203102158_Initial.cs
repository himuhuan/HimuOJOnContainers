using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HimuOJ.Services.Problems.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "problems");

            migrationBuilder.CreateSequence(
                name: "problemseq",
                schema: "problems",
                incrementBy: 10);

            migrationBuilder.CreateSequence(
                name: "testpointseq",
                schema: "problems",
                incrementBy: 10);

            migrationBuilder.CreateTable(
                name: "t_problems",
                schema: "problems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    DistributorId = table.Column<Guid>(type: "uuid", maxLength: 100, nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifyTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DefaultResourceLimit_MaxMemoryLimitByte = table.Column<long>(type: "bigint", nullable: false),
                    DefaultResourceLimit_MaxRealTimeLimitMilliseconds = table.Column<long>(type: "bigint", nullable: false),
                    GuestAccessLimit_AllowDownloadInput = table.Column<bool>(type: "boolean", nullable: false),
                    GuestAccessLimit_AllowDownloadOutput = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_problems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "t_testpoints",
                schema: "problems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    ProblemId = table.Column<int>(type: "integer", nullable: false),
                    Input = table.Column<string>(type: "text", nullable: true),
                    ExpectedOutput = table.Column<string>(type: "text", nullable: true),
                    Remarks = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_testpoints", x => x.Id);
                    table.ForeignKey(
                        name: "FK_t_testpoints_t_problems_ProblemId",
                        column: x => x.ProblemId,
                        principalSchema: "problems",
                        principalTable: "t_problems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_t_problems_DistributorId",
                schema: "problems",
                table: "t_problems",
                column: "DistributorId");

            migrationBuilder.CreateIndex(
                name: "IX_t_testpoints_ProblemId",
                schema: "problems",
                table: "t_testpoints",
                column: "ProblemId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "t_testpoints",
                schema: "problems");

            migrationBuilder.DropTable(
                name: "t_problems",
                schema: "problems");

            migrationBuilder.DropSequence(
                name: "problemseq",
                schema: "problems");

            migrationBuilder.DropSequence(
                name: "testpointseq",
                schema: "problems");
        }
    }
}
