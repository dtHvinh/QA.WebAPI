using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddDiscriminatorForUpvoteAndDownote_170125_0753AM : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Upvote_UpvotedEntityId",
                table: "Upvote");

            migrationBuilder.DropIndex(
                name: "IX_Downvote_DownvotedEntityId",
                table: "Downvote");

            migrationBuilder.DropColumn(
                name: "UpvotedEntityId",
                table: "Upvote");

            migrationBuilder.DropColumn(
                name: "DownvotedEntityId",
                table: "Downvote");

            migrationBuilder.AlterColumn<string>(
                name: "UpvoteType",
                table: "Upvote",
                type: "nvarchar(8)",
                maxLength: 8,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<Guid>(
                name: "AnswerId",
                table: "Upvote",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DownvoteType",
                table: "Downvote",
                type: "nvarchar(8)",
                maxLength: 8,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<Guid>(
                name: "AnswerId",
                table: "Downvote",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Upvote_AnswerId",
                table: "Upvote",
                column: "AnswerId");

            migrationBuilder.CreateIndex(
                name: "IX_Downvote_AnswerId",
                table: "Downvote",
                column: "AnswerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Downvote_Answer_AnswerId",
                table: "Downvote",
                column: "AnswerId",
                principalTable: "Answer",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Upvote_Answer_AnswerId",
                table: "Upvote",
                column: "AnswerId",
                principalTable: "Answer",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Downvote_Answer_AnswerId",
                table: "Downvote");

            migrationBuilder.DropForeignKey(
                name: "FK_Upvote_Answer_AnswerId",
                table: "Upvote");

            migrationBuilder.DropIndex(
                name: "IX_Upvote_AnswerId",
                table: "Upvote");

            migrationBuilder.DropIndex(
                name: "IX_Downvote_AnswerId",
                table: "Downvote");

            migrationBuilder.DropColumn(
                name: "AnswerId",
                table: "Upvote");

            migrationBuilder.DropColumn(
                name: "AnswerId",
                table: "Downvote");

            migrationBuilder.AlterColumn<int>(
                name: "UpvoteType",
                table: "Upvote",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(8)",
                oldMaxLength: 8);

            migrationBuilder.AddColumn<Guid>(
                name: "UpvotedEntityId",
                table: "Upvote",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<int>(
                name: "DownvoteType",
                table: "Downvote",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(8)",
                oldMaxLength: 8);

            migrationBuilder.AddColumn<Guid>(
                name: "DownvotedEntityId",
                table: "Downvote",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Upvote_UpvotedEntityId",
                table: "Upvote",
                column: "UpvotedEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_Downvote_DownvotedEntityId",
                table: "Downvote",
                column: "DownvotedEntityId");
        }
    }
}
