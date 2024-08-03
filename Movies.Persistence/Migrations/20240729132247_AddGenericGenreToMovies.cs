using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Movies.Persistence.Migrations
{
    public partial class AddGenericGenreToMovies : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MovieGenres_Genres_GenreGenericId",
                table: "MovieGenres");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MovieGenres",
                table: "MovieGenres");

            migrationBuilder.DropIndex(
                name: "IX_MovieGenres_GenreGenericId",
                table: "MovieGenres");

            migrationBuilder.DropColumn(
                name: "GenreGenericId",
                table: "MovieGenres");

            migrationBuilder.AddColumn<int>(
                name: "GenreId",
                table: "Movies",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddPrimaryKey(
                name: "PK_MovieGenres",
                table: "MovieGenres",
                columns: new[] { "MovieId", "GenreId" });

            migrationBuilder.CreateIndex(
                name: "IX_Movies_GenreId",
                table: "Movies",
                column: "GenreId");

            migrationBuilder.AddForeignKey(
                name: "FK_Movies_Genres_GenreId",
                table: "Movies",
                column: "GenreId",
                principalTable: "Genres",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Movies_Genres_GenreId",
                table: "Movies");

            migrationBuilder.DropIndex(
                name: "IX_Movies_GenreId",
                table: "Movies");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MovieGenres",
                table: "MovieGenres");

            migrationBuilder.DropColumn(
                name: "GenreId",
                table: "Movies");

            migrationBuilder.AddColumn<int>(
                name: "GenreGenericId",
                table: "MovieGenres",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_MovieGenres",
                table: "MovieGenres",
                columns: new[] { "MovieId", "GenreId", "GenreGenericId" });

            migrationBuilder.CreateIndex(
                name: "IX_MovieGenres_GenreGenericId",
                table: "MovieGenres",
                column: "GenreGenericId");

            migrationBuilder.AddForeignKey(
                name: "FK_MovieGenres_Genres_GenreGenericId",
                table: "MovieGenres",
                column: "GenreGenericId",
                principalTable: "Genres",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
