using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAPI.Migrations
{
    /// <inheritdoc />
    public partial class Updatekjada : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.CreateIndex(
                name: "IX_SuggestEdit_QuestionVersionId",
                table: "SuggestEdit",
                column: "QuestionVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionVersion_QuestionId",
                table: "QuestionVersion",
                column: "QuestionId");

            migrationBuilder.AddForeignKey(
                name: "FK_SuggestEdit_QuestionVersion_QuestionVersionId",
                table: "SuggestEdit",
                column: "QuestionVersionId",
                principalTable: "QuestionVersion",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SuggestEdit_QuestionVersion_QuestionVersionId",
                table: "SuggestEdit");

            migrationBuilder.DropTable(
                name: "QuestionVersion");

            migrationBuilder.DropIndex(
                name: "IX_SuggestEdit_QuestionVersionId",
                table: "SuggestEdit");

            migrationBuilder.DropColumn(
                name: "QuestionVersionId",
                table: "SuggestEdit");
        }
    }
}
