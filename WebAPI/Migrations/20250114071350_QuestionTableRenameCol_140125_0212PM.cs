using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAPI.Migrations
{
    /// <inheritdoc />
    public partial class QuestionTableRenameCol_140125_0212PM : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Question_IsClosed_IsHide_IsDeleted",
                table: "Question");

            migrationBuilder.RenameColumn(
                name: "IsHide",
                table: "Question",
                newName: "IsDraft");

            migrationBuilder.CreateIndex(
                name: "IX_Question_IsDraft_IsClosed_IsDeleted",
                table: "Question",
                columns: new[] { "IsDraft", "IsClosed", "IsDeleted" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Question_IsDraft_IsClosed_IsDeleted",
                table: "Question");

            migrationBuilder.RenameColumn(
                name: "IsDraft",
                table: "Question",
                newName: "IsHide");

            migrationBuilder.CreateIndex(
                name: "IX_Question_IsClosed_IsHide_IsDeleted",
                table: "Question",
                columns: new[] { "IsClosed", "IsHide", "IsDeleted" });
        }
    }
}
