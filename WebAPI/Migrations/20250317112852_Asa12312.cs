using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAPI.Migrations
{
    /// <inheritdoc />
    public partial class Asa12312 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatRoomMessage_AspNetUsers_UserId",
                table: "ChatRoomMessage");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "ChatRoomMessage",
                newName: "AuthorId");

            migrationBuilder.RenameIndex(
                name: "IX_ChatRoomMessage_UserId",
                table: "ChatRoomMessage",
                newName: "IX_ChatRoomMessage_AuthorId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatRoomMessage_AspNetUsers_AuthorId",
                table: "ChatRoomMessage",
                column: "AuthorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatRoomMessage_AspNetUsers_AuthorId",
                table: "ChatRoomMessage");

            migrationBuilder.RenameColumn(
                name: "AuthorId",
                table: "ChatRoomMessage",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_ChatRoomMessage_AuthorId",
                table: "ChatRoomMessage",
                newName: "IX_ChatRoomMessage_UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatRoomMessage_AspNetUsers_UserId",
                table: "ChatRoomMessage",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
