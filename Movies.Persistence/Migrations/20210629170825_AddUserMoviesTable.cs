using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Movies.Persistence.Migrations
{
    public partial class AddUserMoviesTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {


            migrationBuilder.CreateTable(
                name: "UserMovies",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MovieId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserMovies", x => new { x.UserId, x.MovieId });
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserMovies_UserId_MovieId",
                table: "UserMovies",
                columns: new[] { "UserId", "MovieId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserMovies");


        }
    }
}
