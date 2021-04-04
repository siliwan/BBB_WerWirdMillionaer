using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Backend.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"CREATE VIEW [dbo].[VHighScore] AS
                                        SELECT
											[Id],
											[Name],
											[TimeStamp],
											[PointsAchieved],
											[Duration],
											[PointsWeighted],
											STUFF(
												(SELECT 
													',' + [dbo].[Categories].[Name]
												 FROM
													[dbo].[Categories]
													INNER JOIN [dbo].[CategoryHighscore] ON [dbo].[Categories].[Id] = [dbo].[CategoryHighscore].[CategoriesId]
												 WHERE
													[dbo].[Highscores].[Id] = [dbo].[CategoryHighscore].[HighscoresId]
												 FOR XML PATH ('')),
												 1,
												 1,
												 ''
											) AS [Categories],
											RANK() OVER (ORDER BY PointsWeighted DESC) AS [Rank]
										FROM
											[dbo].[Highscores]");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP VIEW IF EXISTS [dbo].[VHighScore]");
        }
    }
}
