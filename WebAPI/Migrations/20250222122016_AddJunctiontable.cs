using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddJunctiontable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CollectionLike_QuestionCollection_CollectionId",
                table: "CollectionLike");

            migrationBuilder.DropTable(
                name: "QuestionQuestionCollection");

            migrationBuilder.DropTable(
                name: "QuestionCollection");

            migrationBuilder.CreateTable(
                name: "Collection",
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
                    table.PrimaryKey("PK_Collection", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Collection_AspNetUsers_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CollectionQuestion",
                columns: table => new
                {
                    CollectionsId = table.Column<int>(type: "int", nullable: false),
                    QuestionsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CollectionQuestion", x => new { x.CollectionsId, x.QuestionsId });
                    table.ForeignKey(
                        name: "FK_CollectionQuestion_Collection_CollectionsId",
                        column: x => x.CollectionsId,
                        principalTable: "Collection",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CollectionQuestion_Question_QuestionsId",
                        column: x => x.QuestionsId,
                        principalTable: "Question",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Collection_AuthorId",
                table: "Collection",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Collection_CreatedAt",
                table: "Collection",
                column: "CreatedAt",
                descending: new bool[0]);

            migrationBuilder.CreateIndex(
                name: "IX_Collection_IsPublic",
                table: "Collection",
                column: "IsPublic");

            migrationBuilder.CreateIndex(
                name: "IX_Collection_LikeCount",
                table: "Collection",
                column: "LikeCount");

            migrationBuilder.CreateIndex(
                name: "IX_CollectionQuestion_QuestionsId",
                table: "CollectionQuestion",
                column: "QuestionsId");

            migrationBuilder.AddForeignKey(
                name: "FK_CollectionLike_Collection_CollectionId",
                table: "CollectionLike",
                column: "CollectionId",
                principalTable: "Collection",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CollectionLike_Collection_CollectionId",
                table: "CollectionLike");

            migrationBuilder.DropTable(
                name: "CollectionQuestion");

            migrationBuilder.DropTable(
                name: "Collection");

            migrationBuilder.CreateTable(
                name: "QuestionCollection",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AuthorId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsPublic = table.Column<bool>(type: "bit", nullable: false),
                    LikeCount = table.Column<int>(type: "int", nullable: false),
                    nvarchar255 = table.Column<string>(name: "nvarchar(255)", type: "nvarchar(max)", nullable: false),
                    QuestionCount = table.Column<int>(type: "int", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
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
                name: "IX_QuestionCollection_AuthorId",
                table: "QuestionCollection",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionCollection_CreatedAt",
                table: "QuestionCollection",
                column: "CreatedAt",
                descending: new bool[0]);

            migrationBuilder.CreateIndex(
                name: "IX_QuestionCollection_IsPublic",
                table: "QuestionCollection",
                column: "IsPublic");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionCollection_LikeCount",
                table: "QuestionCollection",
                column: "LikeCount");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionQuestionCollection_QuestionsId",
                table: "QuestionQuestionCollection",
                column: "QuestionsId");

            migrationBuilder.AddForeignKey(
                name: "FK_CollectionLike_QuestionCollection_CollectionId",
                table: "CollectionLike",
                column: "CollectionId",
                principalTable: "QuestionCollection",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
