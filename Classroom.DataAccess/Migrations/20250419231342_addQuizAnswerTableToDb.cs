using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Classroom.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class addQuizAnswerTableToDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuizQuestions_QuizSubmissions_QuizSubmissionId",
                table: "QuizQuestions");

            migrationBuilder.DropIndex(
                name: "IX_QuizQuestions_QuizSubmissionId",
                table: "QuizQuestions");

            migrationBuilder.DropColumn(
                name: "QuizSubmissionId",
                table: "QuizQuestions");

            migrationBuilder.CreateTable(
                name: "QuizAnswers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuestionId = table.Column<int>(type: "int", nullable: false),
                    AnswerText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    QuizSubmissionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuizAnswers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuizAnswers_QuizSubmissions_QuizSubmissionId",
                        column: x => x.QuizSubmissionId,
                        principalTable: "QuizSubmissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_QuizAnswers_QuizSubmissionId",
                table: "QuizAnswers",
                column: "QuizSubmissionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "QuizAnswers");

            migrationBuilder.AddColumn<int>(
                name: "QuizSubmissionId",
                table: "QuizQuestions",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_QuizQuestions_QuizSubmissionId",
                table: "QuizQuestions",
                column: "QuizSubmissionId");

            migrationBuilder.AddForeignKey(
                name: "FK_QuizQuestions_QuizSubmissions_QuizSubmissionId",
                table: "QuizQuestions",
                column: "QuizSubmissionId",
                principalTable: "QuizSubmissions",
                principalColumn: "Id");
        }
    }
}
