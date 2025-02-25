using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAPI.Migrations
{
    /// <inheritdoc />
    public partial class Updatekjadaasd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SuggestEdit_QuestionVersion_QuestionVersionId",
                table: "SuggestEdit");

            migrationBuilder.DropTable(
                name: "QuestionVersion");

            migrationBuilder.DropTable(
                name: "RollBackAction");

            migrationBuilder.DropIndex(
                name: "IX_SuggestEdit_QuestionVersionId",
                table: "SuggestEdit");

            migrationBuilder.DropColumn(
                name: "QuestionVersionId",
                table: "SuggestEdit");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "QuestionVersionId",
                table: "SuggestEdit",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "QuestionVersion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuestionId = table.Column<int>(type: "int", nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionVersion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuestionVersion_Question_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Question",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RollBackAction",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AuthorId = table.Column<int>(type: "int", nullable: false),
                    SuggestEditId = table.Column<int>(type: "int", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RollBackAction", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RollBackAction_AspNetUsers_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RollBackAction_SuggestEdit_SuggestEditId",
                        column: x => x.SuggestEditId,
                        principalTable: "SuggestEdit",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SuggestEdit_QuestionVersionId",
                table: "SuggestEdit",
                column: "QuestionVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionVersion_QuestionId",
                table: "QuestionVersion",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_RollBackAction_AuthorId",
                table: "RollBackAction",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_RollBackAction_SuggestEditId",
                table: "RollBackAction",
                column: "SuggestEditId");

            migrationBuilder.AddForeignKey(
                name: "FK_SuggestEdit_QuestionVersion_QuestionVersionId",
                table: "SuggestEdit",
                column: "QuestionVersionId",
                principalTable: "QuestionVersion",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
