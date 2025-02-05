using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdateQuestionIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Question_IsDraft_IsClosed_IsDeleted",
                table: "Question");

            migrationBuilder.CreateIndex(
                name: "IX_Question_IsDraft_IsClosed_IsDeleted_IsSolved_AuthorId_ViewCount",
                table: "Question",
                columns: new[] { "IsDraft", "IsClosed", "IsDeleted", "IsSolved", "AuthorId", "ViewCount" },
                descending: new bool[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Question_IsDraft_IsClosed_IsDeleted_IsSolved_AuthorId_ViewCount",
                table: "Question");

            migrationBuilder.CreateIndex(
                name: "IX_Question_IsDraft_IsClosed_IsDeleted",
                table: "Question",
                columns: new[] { "IsDraft", "IsClosed", "IsDeleted" });
        }
    }
}
