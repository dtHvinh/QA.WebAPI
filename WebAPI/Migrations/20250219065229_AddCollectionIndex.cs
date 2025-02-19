using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddCollectionIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_QuestionCollection_CreatedAt",
                table: "QuestionCollection",
                column: "CreatedAt",
                descending: new bool[0]);

            migrationBuilder.CreateIndex(
                name: "IX_QuestionCollection_IsPublic",
                table: "QuestionCollection",
                column: "IsPublic");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionCollection_LikeCount",
                table: "QuestionCollection",
                column: "LikeCount");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_QuestionCollection_CreatedAt",
                table: "QuestionCollection");

            migrationBuilder.DropIndex(
                name: "IX_QuestionCollection_IsPublic",
                table: "QuestionCollection");

            migrationBuilder.DropIndex(
                name: "IX_QuestionCollection_LikeCount",
                table: "QuestionCollection");
        }
    }
}
