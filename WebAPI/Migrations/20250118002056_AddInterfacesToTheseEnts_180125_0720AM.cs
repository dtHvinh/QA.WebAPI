using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddInterfacesToTheseEnts_180125_0720AM : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookMark_AspNetUsers_UserId",
                table: "BookMark");

            migrationBuilder.DropForeignKey(
                name: "FK_Report_AspNetUsers_ReporterId",
                table: "Report");

            migrationBuilder.DropForeignKey(
                name: "FK_Upvote_AspNetUsers_UserId",
                table: "Upvote");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Upvote",
                newName: "AuthorId");

            migrationBuilder.RenameIndex(
                name: "IX_Upvote_UserId",
                table: "Upvote",
                newName: "IX_Upvote_AuthorId");

            migrationBuilder.RenameColumn(
                name: "ReporterId",
                table: "Report",
                newName: "AuthorId");

            migrationBuilder.RenameIndex(
                name: "IX_Report_ReporterId",
                table: "Report",
                newName: "IX_Report_AuthorId");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "BookMark",
                newName: "AuthorId");

            migrationBuilder.RenameIndex(
                name: "IX_BookMark_UserId_CreatedAt",
                table: "BookMark",
                newName: "IX_BookMark_AuthorId_CreatedAt");

            migrationBuilder.AddForeignKey(
                name: "FK_BookMark_AspNetUsers_AuthorId",
                table: "BookMark",
                column: "AuthorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Report_AspNetUsers_AuthorId",
                table: "Report",
                column: "AuthorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Upvote_AspNetUsers_AuthorId",
                table: "Upvote",
                column: "AuthorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookMark_AspNetUsers_AuthorId",
                table: "BookMark");

            migrationBuilder.DropForeignKey(
                name: "FK_Report_AspNetUsers_AuthorId",
                table: "Report");

            migrationBuilder.DropForeignKey(
                name: "FK_Upvote_AspNetUsers_AuthorId",
                table: "Upvote");

            migrationBuilder.RenameColumn(
                name: "AuthorId",
                table: "Upvote",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Upvote_AuthorId",
                table: "Upvote",
                newName: "IX_Upvote_UserId");

            migrationBuilder.RenameColumn(
                name: "AuthorId",
                table: "Report",
                newName: "ReporterId");

            migrationBuilder.RenameIndex(
                name: "IX_Report_AuthorId",
                table: "Report",
                newName: "IX_Report_ReporterId");

            migrationBuilder.RenameColumn(
                name: "AuthorId",
                table: "BookMark",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_BookMark_AuthorId_CreatedAt",
                table: "BookMark",
                newName: "IX_BookMark_UserId_CreatedAt");

            migrationBuilder.AddForeignKey(
                name: "FK_BookMark_AspNetUsers_UserId",
                table: "BookMark",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Report_AspNetUsers_ReporterId",
                table: "Report",
                column: "ReporterId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Upvote_AspNetUsers_UserId",
                table: "Upvote",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
