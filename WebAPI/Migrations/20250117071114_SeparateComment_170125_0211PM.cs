using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAPI.Migrations
{
    /// <inheritdoc />
    public partial class SeparateComment_170125_0211PM : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuestionComment_AspNetUsers_AuthorId",
                table: "QuestionComment");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionComment_Question_QuestionId",
                table: "QuestionComment");

            migrationBuilder.DropPrimaryKey(
                name: "PK_QuestionComment",
                table: "QuestionComment");

            migrationBuilder.RenameTable(
                name: "QuestionComment",
                newName: "Comment");

            migrationBuilder.RenameIndex(
                name: "IX_QuestionComment_QuestionId",
                table: "Comment",
                newName: "IX_Comment_QuestionId");

            migrationBuilder.RenameIndex(
                name: "IX_QuestionComment_AuthorId",
                table: "Comment",
                newName: "IX_Comment_AuthorId");

            migrationBuilder.AlterColumn<Guid>(
                name: "QuestionId",
                table: "Comment",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "AnswerId",
                table: "Comment",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CommentType",
                table: "Comment",
                type: "nvarchar(8)",
                maxLength: 8,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Comment",
                table: "Comment",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Comment_AnswerId",
                table: "Comment",
                column: "AnswerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_Answer_AnswerId",
                table: "Comment",
                column: "AnswerId",
                principalTable: "Answer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_AspNetUsers_AuthorId",
                table: "Comment",
                column: "AuthorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_Question_QuestionId",
                table: "Comment",
                column: "QuestionId",
                principalTable: "Question",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comment_Answer_AnswerId",
                table: "Comment");

            migrationBuilder.DropForeignKey(
                name: "FK_Comment_AspNetUsers_AuthorId",
                table: "Comment");

            migrationBuilder.DropForeignKey(
                name: "FK_Comment_Question_QuestionId",
                table: "Comment");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Comment",
                table: "Comment");

            migrationBuilder.DropIndex(
                name: "IX_Comment_AnswerId",
                table: "Comment");

            migrationBuilder.DropColumn(
                name: "AnswerId",
                table: "Comment");

            migrationBuilder.DropColumn(
                name: "CommentType",
                table: "Comment");

            migrationBuilder.RenameTable(
                name: "Comment",
                newName: "QuestionComment");

            migrationBuilder.RenameIndex(
                name: "IX_Comment_QuestionId",
                table: "QuestionComment",
                newName: "IX_QuestionComment_QuestionId");

            migrationBuilder.RenameIndex(
                name: "IX_Comment_AuthorId",
                table: "QuestionComment",
                newName: "IX_QuestionComment_AuthorId");

            migrationBuilder.AlterColumn<Guid>(
                name: "QuestionId",
                table: "QuestionComment",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_QuestionComment",
                table: "QuestionComment",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionComment_AspNetUsers_AuthorId",
                table: "QuestionComment",
                column: "AuthorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionComment_Question_QuestionId",
                table: "QuestionComment",
                column: "QuestionId",
                principalTable: "Question",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
