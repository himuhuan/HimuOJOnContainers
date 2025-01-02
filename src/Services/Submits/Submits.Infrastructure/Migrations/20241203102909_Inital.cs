using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HimuOJ.Services.Submits.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Inital : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "submits");

            migrationBuilder.CreateSequence(
                name: "submissionseq",
                schema: "submits",
                incrementBy: 10);

            migrationBuilder.CreateSequence(
                name: "testpointresultseq",
                schema: "submits",
                incrementBy: 10);

            migrationBuilder.CreateTable(
                name: "t_submissions",
                schema: "submits",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    ProblemId = table.Column<int>(type: "integer", nullable: true),
                    SubmitterId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    SourceCode = table.Column<string>(type: "character varying(10000)", maxLength: 10000, nullable: false),
                    SubmitTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CompilerName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    UsedMemoryByte = table.Column<long>(type: "bigint", nullable: true),
                    UsedTimeMs = table.Column<long>(type: "bigint", nullable: true),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    StatusMessage = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_submissions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "t_testpoint_results",
                schema: "submits",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    SubmissionId = table.Column<int>(type: "integer", nullable: false),
                    TestPointId = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    UsedMemoryByte = table.Column<long>(type: "bigint", nullable: false),
                    UsedTimeMs = table.Column<long>(type: "bigint", nullable: false),
                    ExpectedOutput = table.Column<string>(type: "character varying(10000)", maxLength: 10000, nullable: true),
                    ActualOutput = table.Column<string>(type: "character varying(10000)", maxLength: 10000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_testpoint_results", x => x.Id);
                    table.ForeignKey(
                        name: "FK_t_testpoint_results_t_submissions_SubmissionId",
                        column: x => x.SubmissionId,
                        principalSchema: "submits",
                        principalTable: "t_submissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_t_submissions_CompilerName",
                schema: "submits",
                table: "t_submissions",
                column: "CompilerName");

            migrationBuilder.CreateIndex(
                name: "IX_t_submissions_ProblemId",
                schema: "submits",
                table: "t_submissions",
                column: "ProblemId");

            migrationBuilder.CreateIndex(
                name: "IX_t_submissions_Status",
                schema: "submits",
                table: "t_submissions",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_t_submissions_SubmitterId",
                schema: "submits",
                table: "t_submissions",
                column: "SubmitterId");

            migrationBuilder.CreateIndex(
                name: "IX_t_submissions_SubmitTime",
                schema: "submits",
                table: "t_submissions",
                column: "SubmitTime");

            migrationBuilder.CreateIndex(
                name: "IX_t_testpoint_results_Status",
                schema: "submits",
                table: "t_testpoint_results",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_t_testpoint_results_SubmissionId",
                schema: "submits",
                table: "t_testpoint_results",
                column: "SubmissionId");

            migrationBuilder.CreateIndex(
                name: "IX_t_testpoint_results_TestPointId",
                schema: "submits",
                table: "t_testpoint_results",
                column: "TestPointId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "t_testpoint_results",
                schema: "submits");

            migrationBuilder.DropTable(
                name: "t_submissions",
                schema: "submits");

            migrationBuilder.DropSequence(
                name: "submissionseq",
                schema: "submits");

            migrationBuilder.DropSequence(
                name: "testpointresultseq",
                schema: "submits");
        }
    }
}
