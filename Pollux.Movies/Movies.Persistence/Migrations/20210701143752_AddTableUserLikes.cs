using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Movies.Persistence.Migrations
{
    public partial class AddTableUserLikes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserLikes",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MovieId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLikes", x => new { x.UserId, x.MovieId });
                    table.ForeignKey(
                        name: "FK_UserLikes_Movies_MovieId",
                        column: x => x.MovieId,
                        principalTable: "Movies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserMovies_MovieId",
                table: "UserMovies",
                column: "MovieId");

            migrationBuilder.CreateIndex(
                name: "IX_UserLikes_MovieId",
                table: "UserLikes",
                column: "MovieId");

            migrationBuilder.CreateIndex(
                name: "IX_UserLikes_UserId_MovieId",
                table: "UserLikes",
                columns: new[] { "UserId", "MovieId" });

            migrationBuilder.AddForeignKey(
                name: "FK_UserMovies_Movies_MovieId",
                table: "UserMovies",
                column: "MovieId",
                principalTable: "Movies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserMovies_Movies_MovieId",
                table: "UserMovies");

            migrationBuilder.DropTable(
                name: "UserLikes");

            migrationBuilder.DropIndex(
                name: "IX_UserMovies_MovieId",
                table: "UserMovies");
        }
    }
}
