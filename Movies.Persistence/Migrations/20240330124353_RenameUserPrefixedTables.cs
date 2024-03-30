using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Movies.Persistence.Migrations
{
    public partial class RenameUserPrefixedTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserLikes");

            migrationBuilder.DropTable(
                name: "UserMovies");

            migrationBuilder.DropIndex(
                name: "IX_Movies_Name_Gender_Language_Likes_Recommended_Imbd",
                table: "Movies");

            migrationBuilder.CreateTable(
                name: "MoviesLikes",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MovieId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MoviesLikes", x => new { x.UserId, x.MovieId });
                    table.ForeignKey(
                        name: "FK_MoviesLikes_Movies_MovieId",
                        column: x => x.MovieId,
                        principalTable: "Movies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MoviesLists",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MovieId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MoviesLists", x => new { x.UserId, x.MovieId });
                    table.ForeignKey(
                        name: "FK_MoviesLists_Movies_MovieId",
                        column: x => x.MovieId,
                        principalTable: "Movies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Movies_Name_Language_Likes_Recommended_Imbd",
                table: "Movies",
                columns: new[] { "Name", "Language", "Likes", "Recommended", "Imbd" });

            migrationBuilder.CreateIndex(
                name: "IX_MoviesLikes_MovieId",
                table: "MoviesLikes",
                column: "MovieId");

            migrationBuilder.CreateIndex(
                name: "IX_MoviesLikes_UserId_MovieId",
                table: "MoviesLikes",
                columns: new[] { "UserId", "MovieId" });

            migrationBuilder.CreateIndex(
                name: "IX_MoviesLists_MovieId",
                table: "MoviesLists",
                column: "MovieId");

            migrationBuilder.CreateIndex(
                name: "IX_MoviesLists_UserId_MovieId",
                table: "MoviesLists",
                columns: new[] { "UserId", "MovieId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MoviesLikes");

            migrationBuilder.DropTable(
                name: "MoviesLists");

            migrationBuilder.DropIndex(
                name: "IX_Movies_Name_Language_Likes_Recommended_Imbd",
                table: "Movies");

            migrationBuilder.CreateTable(
                name: "UserLikes",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MovieId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLikes", x => new { x.UserId, x.MovieId });
                    table.ForeignKey(
                        name: "FK_UserLikes_Movies_MovieId",
                        column: x => x.MovieId,
                        principalTable: "Movies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserMovies",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MovieId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserMovies", x => new { x.UserId, x.MovieId });
                    table.ForeignKey(
                        name: "FK_UserMovies_Movies_MovieId",
                        column: x => x.MovieId,
                        principalTable: "Movies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Movies_Name_Gender_Language_Likes_Recommended_Imbd",
                table: "Movies",
                columns: new[] { "Name", "Gender", "Language", "Likes", "Recommended", "Imbd" });

            migrationBuilder.CreateIndex(
                name: "IX_UserLikes_MovieId",
                table: "UserLikes",
                column: "MovieId");

            migrationBuilder.CreateIndex(
                name: "IX_UserLikes_UserId_MovieId",
                table: "UserLikes",
                columns: new[] { "UserId", "MovieId" });

            migrationBuilder.CreateIndex(
                name: "IX_UserMovies_MovieId",
                table: "UserMovies",
                column: "MovieId");

            migrationBuilder.CreateIndex(
                name: "IX_UserMovies_UserId_MovieId",
                table: "UserMovies",
                columns: new[] { "UserId", "MovieId" });
        }
    }
}
