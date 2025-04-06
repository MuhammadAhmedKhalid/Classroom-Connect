using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Classroom.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class addCreationMetadataToClassTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Classes",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatedById",
                table: "Classes",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Classes_CreatedById",
                table: "Classes",
                column: "CreatedById");

            migrationBuilder.AddForeignKey(
                name: "FK_Classes_AspNetUsers_CreatedById",
                table: "Classes",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Classes_AspNetUsers_CreatedById",
                table: "Classes");

            migrationBuilder.DropIndex(
                name: "IX_Classes_CreatedById",
                table: "Classes");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Classes");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "Classes");
        }
    }
}
