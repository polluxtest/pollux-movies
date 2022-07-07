using Microsoft.EntityFrameworkCore.Migrations;
using System;

#nullable disable

namespace Movies.Persistence.Migrations
{
    public partial class AddContinueWatchingMoviesTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MoviesWatching",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MovieId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ElapsedTime = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    Duration = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovieWatching", x => new { x.UserId, x.MovieId });
                    table.ForeignKey(
                        name: "FK_MovieWatching_Movies_MovieId",
                        column: x => x.MovieId,
                        principalTable: "Movies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MovieWatching_MovieId",
                table: "MoviesWatching",
                column: "MovieId");

            migrationBuilder.CreateIndex(
                name: "IX_MovieWatching_UserId_MovieId",
                table: "MoviesWatching",
                columns: new[] { "UserId", "MovieId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MoviesWatching");
        }
    }
}
