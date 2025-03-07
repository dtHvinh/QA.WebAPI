using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAPI.Migrations
{
    /// <inheritdoc />
    public partial class FromUpvoteDownVoteCountToScore : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Downvotes",
                table: "Question");

            migrationBuilder.DropColumn(
                name: "Downvote",
                table: "Answer");

            migrationBuilder.RenameColumn(
                name: "Upvotes",
                table: "Question",
                newName: "Score");

            migrationBuilder.RenameIndex(
                name: "IX_Question_Upvotes",
                table: "Question",
                newName: "IX_Question_Score");

            migrationBuilder.RenameColumn(
                name: "Upvote",
                table: "Answer",
                newName: "Score");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Score",
                table: "Question",
                newName: "Upvotes");

            migrationBuilder.RenameIndex(
                name: "IX_Question_Score",
                table: "Question",
                newName: "IX_Question_Upvotes");

            migrationBuilder.RenameColumn(
                name: "Score",
                table: "Answer",
                newName: "Upvote");

            migrationBuilder.AddColumn<int>(
                name: "Downvotes",
                table: "Question",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Downvote",
                table: "Answer",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
