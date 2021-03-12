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
	                                        RANK() OVER (ORDER BY PointsWeighted) AS [Rank]
                                        FROM
	                                        [dbo].[Highscores]");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP VIEW IF EXISTS [dbo].[VHighScore]");
        }
    }
}
