using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Classroom.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class addClassMembersTableToDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClassesJoined");

            migrationBuilder.CreateTable(
                name: "ClassMembers",
                columns: table => new
                {
                    ClassMemberId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClassId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    JoinedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassMembers", x => x.ClassMemberId);
                    table.ForeignKey(
                        name: "FK_ClassMembers_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ClassMembers_Classes_ClassId",
                        column: x => x.ClassId,
                        principalTable: "Classes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClassMembers_ClassId",
                table: "ClassMembers",
                column: "ClassId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassMembers_UserId",
                table: "ClassMembers",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClassMembers");

            migrationBuilder.CreateTable(
                name: "ClassesJoined",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClassId = table.Column<int>(type: "int", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ClassCode = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                    JoinDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassesJoined", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClassesJoined_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ClassesJoined_Classes_ClassId",
                        column: x => x.ClassId,
                        principalTable: "Classes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClassesJoined_ClassId",
                table: "ClassesJoined",
                column: "ClassId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassesJoined_UserId",
                table: "ClassesJoined",
                column: "UserId");
        }
    }
}
