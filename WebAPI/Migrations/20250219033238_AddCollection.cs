using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddCollection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "QuestionCollection",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AuthorId = table.Column<int>(type: "int", nullable: false),
                    nvarchar255 = table.Column<string>(name: "nvarchar(255)", type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsPublic = table.Column<bool>(type: "bit", nullable: false),
                    QuestionCount = table.Column<int>(type: "int", nullable: false),
                    LikeCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionCollection", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuestionCollection_AspNetUsers_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CollectionLike",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AuthorId = table.Column<int>(type: "int", nullable: false),
                    CollectionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CollectionLike", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CollectionLike_AspNetUsers_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CollectionLike_QuestionCollection_CollectionId",
                        column: x => x.CollectionId,
                        principalTable: "QuestionCollection",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "QuestionQuestionCollection",
                columns: table => new
                {
                    QuestionCollectionsId = table.Column<int>(type: "int", nullable: false),
                    QuestionsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionQuestionCollection", x => new { x.QuestionCollectionsId, x.QuestionsId });
                    table.ForeignKey(
                        name: "FK_QuestionQuestionCollection_QuestionCollection_QuestionCollectionsId",
                        column: x => x.QuestionCollectionsId,
                        principalTable: "QuestionCollection",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuestionQuestionCollection_Question_QuestionsId",
                        column: x => x.QuestionsId,
                        principalTable: "Question",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CollectionLike_AuthorId",
                table: "CollectionLike",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_CollectionLike_CollectionId",
                table: "CollectionLike",
                column: "CollectionId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionCollection_AuthorId",
                table: "QuestionCollection",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionQuestionCollection_QuestionsId",
                table: "QuestionQuestionCollection",
                column: "QuestionsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CollectionLike");

            migrationBuilder.DropTable(
                name: "QuestionQuestionCollection");

            migrationBuilder.DropTable(
                name: "QuestionCollection");
        }
    }
}
