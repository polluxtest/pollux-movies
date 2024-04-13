using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Movies.Persistence.Migrations
{
    public partial class AddMissingIndexes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MoviesWatching_UserId",
                table: "MoviesWatching");

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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
