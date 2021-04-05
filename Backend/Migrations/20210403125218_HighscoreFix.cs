using Microsoft.EntityFrameworkCore.Migrations;

namespace Backend.Migrations
{
    public partial class HighscoreFix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CategoryHighscore",
                columns: table => new
                {
                    CategoriesId = table.Column<int>(type: "int", nullable: false),
                    HighscoresId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryHighscore", x => new { x.CategoriesId, x.HighscoresId });
                    table.ForeignKey(
                        name: "FK_CategoryHighscore_Categories_CategoriesId",
                        column: x => x.CategoriesId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CategoryHighscore_Highscores_HighscoresId",
                        column: x => x.HighscoresId,
                        principalTable: "Highscores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CategoryHighscore_HighscoresId",
                table: "CategoryHighscore",
                column: "HighscoresId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CategoryHighscore");

            migrationBuilder.AddColumn<int>(
                name: "HighscoreId",
                table: "Categories",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Categories_HighscoreId",
                table: "Categories",
                column: "HighscoreId");

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_Highscores_HighscoreId",
                table: "Categories",
                column: "HighscoreId",
                principalTable: "Highscores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
