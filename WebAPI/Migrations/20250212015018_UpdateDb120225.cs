using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDb120225 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Upvote",
                table: "Question",
                newName: "Upvotes");

            migrationBuilder.RenameColumn(
                name: "Downvote",
                table: "Question",
                newName: "Downvotes");

            migrationBuilder.CreateIndex(
                name: "IX_Question_Upvotes",
                table: "Question",
                column: "Upvotes");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Question_Upvotes",
                table: "Question");

            migrationBuilder.RenameColumn(
                name: "Upvotes",
                table: "Question",
                newName: "Upvote");

            migrationBuilder.RenameColumn(
                name: "Downvotes",
                table: "Question",
                newName: "Downvote");
        }
    }
}
