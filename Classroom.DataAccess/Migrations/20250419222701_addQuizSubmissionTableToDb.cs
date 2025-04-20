using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Classroom.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class addQuizSubmissionTableToDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "QuizSubmissionId",
                table: "QuizQuestions",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "QuizSubmissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuizId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    SubmittedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuizSubmissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuizSubmissions_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_QuizSubmissions_Quizzes_QuizId",
                        column: x => x.QuizId,
                        principalTable: "Quizzes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_QuizQuestions_QuizSubmissionId",
                table: "QuizQuestions",
                column: "QuizSubmissionId");

            migrationBuilder.CreateIndex(
                name: "IX_QuizSubmissions_QuizId",
                table: "QuizSubmissions",
                column: "QuizId");

            migrationBuilder.CreateIndex(
                name: "IX_QuizSubmissions_UserId",
                table: "QuizSubmissions",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_QuizQuestions_QuizSubmissions_QuizSubmissionId",
                table: "QuizQuestions",
                column: "QuizSubmissionId",
                principalTable: "QuizSubmissions",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuizQuestions_QuizSubmissions_QuizSubmissionId",
                table: "QuizQuestions");

            migrationBuilder.DropTable(
                name: "QuizSubmissions");

            migrationBuilder.DropIndex(
                name: "IX_QuizQuestions_QuizSubmissionId",
                table: "QuizQuestions");

            migrationBuilder.DropColumn(
                name: "QuizSubmissionId",
                table: "QuizQuestions");
        }
    }
}
