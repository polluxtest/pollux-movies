using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Movies.Persistence.Migrations
{
    public partial class recreateMoviesGenres : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
         

            migrationBuilder.CreateTable(
                name: "MovieGenres",
                columns: table => new
                {
                    MovieId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GenreId = table.Column<int>(type: "int", nullable: false),
                    GenreGenericId = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovieGenres", x => new { x.MovieId, x.GenreId, x.GenreGenericId });
                    table.ForeignKey(
                        name: "FK_MovieGenres_Genres_GenreGenericId",
                        column: x => x.GenreGenericId,
                        principalTable: "Genres",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_MovieGenres_Genres_GenreId",
                        column: x => x.GenreId,
                        principalTable: "Genres",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_MovieGenres_Movies_MovieId",
                        column: x => x.MovieId,
                        principalTable: "Movies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MoviesWatching_UserId",
                table: "MoviesWatching",
                column: "UserId")
                .Annotation("SqlServer:Include", new[] { "Duration", "ElapsedTime", "RemainingTime" });

            migrationBuilder.CreateIndex(
                name: "IX_MoviesLists_UserId",
                table: "MoviesLists",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_MoviesLikes_UserId",
                table: "MoviesLikes",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_MovieGenres_GenreGenericId",
                table: "MovieGenres",
                column: "GenreGenericId");

            migrationBuilder.CreateIndex(
                name: "IX_MovieGenres_GenreId",
                table: "MovieGenres",
                column: "GenreId");

            migrationBuilder.CreateIndex(
                name: "IX_MovieGenres_MovieId",
                table: "MovieGenres",
                column: "MovieId");

            migrationBuilder.CreateIndex(
                name: "IX_MovieGenres_MovieId_GenreId",
                table: "MovieGenres",
                columns: new[] { "MovieId", "GenreId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MovieGenres");

            migrationBuilder.DropIndex(
                name: "IX_MoviesWatching_UserId",
                table: "MoviesWatching");

            migrationBuilder.DropIndex(
                name: "IX_MoviesLists_UserId",
                table: "MoviesLists");

            migrationBuilder.DropIndex(
                name: "IX_MoviesLikes_UserId",
                table: "MoviesLikes");

            migrationBuilder.CreateIndex(
                name: "IX_MoviesWatching_UserId",
                table: "MoviesWatching",
                column: "UserId");
        }
    }
}
