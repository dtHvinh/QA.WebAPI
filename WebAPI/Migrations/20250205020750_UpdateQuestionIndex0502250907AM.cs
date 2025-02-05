using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdateQuestionIndex0502250907AM : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Question_IsDraft_IsClosed_IsDeleted_CreatedAt_IsSolved_AuthorId_ViewCount",
                table: "Question");

            migrationBuilder.CreateIndex(
                name: "IX_Question_CreatedAt",
                table: "Question",
                column: "CreatedAt",
                descending: new bool[0]);

            migrationBuilder.CreateIndex(
                name: "IX_Question_IsDraft",
                table: "Question",
                column: "IsDraft");

            migrationBuilder.CreateIndex(
                name: "IX_Question_IsSolved",
                table: "Question",
                column: "IsSolved");

            migrationBuilder.CreateIndex(
                name: "IX_Question_ViewCount",
                table: "Question",
                column: "ViewCount",
                descending: new bool[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Question_CreatedAt",
                table: "Question");

            migrationBuilder.DropIndex(
                name: "IX_Question_IsDraft",
                table: "Question");

            migrationBuilder.DropIndex(
                name: "IX_Question_IsSolved",
                table: "Question");

            migrationBuilder.DropIndex(
                name: "IX_Question_ViewCount",
                table: "Question");

            migrationBuilder.CreateIndex(
                name: "IX_Question_IsDraft_IsClosed_IsDeleted_CreatedAt_IsSolved_AuthorId_ViewCount",
                table: "Question",
                columns: new[] { "IsDraft", "IsClosed", "IsDeleted", "CreatedAt", "IsSolved", "AuthorId", "ViewCount" },
                descending: new bool[0]);
        }
    }
}
