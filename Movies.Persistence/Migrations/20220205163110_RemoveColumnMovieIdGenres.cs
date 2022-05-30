using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Movies.Persistence.Migrations
{
    public partial class RemoveColumnMovieIdGenres : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Genres_Movies_MovieId",
                table: "Genres");

            migrationBuilder.DropIndex(
                name: "IX_Genres_MovieId",
                table: "Genres");

            migrationBuilder.DropColumn(
                name: "MovieId",
                table: "Genres");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "MovieId",
                table: "Genres",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Genres_MovieId",
                table: "Genres",
                column: "MovieId");

            migrationBuilder.AddForeignKey(
                name: "FK_Genres_Movies_MovieId",
                table: "Genres",
                column: "MovieId",
                principalTable: "Movies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
