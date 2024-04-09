using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Movies.Persistence.Migrations
{
    public partial class AddMovieIndexes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DropColumn(
                name: "ProcessedByAzureJob",
                table: "Movies");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Movies",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<bool>(
                name: "ProcessedByStreamVideo",
                table: "Movies",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_MoviesWatching_UserId",
                table: "MoviesWatching",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Movies_Id",
                table: "Movies",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Movies_Imbd",
                table: "Movies",
                column: "Imbd");

            migrationBuilder.CreateIndex(
                name: "IX_Movies_IsDeleted_ProcessedByStreamVideo",
                table: "Movies",
                columns: new[] { "IsDeleted", "ProcessedByStreamVideo" },
                filter: "[IsDeleted] = 0 and [ProcessedByStreamVideo] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_Movies_Language",
                table: "Movies",
                column: "Language");

            migrationBuilder.CreateIndex(
                name: "IX_Movies_Likes",
                table: "Movies",
                column: "Likes");

            migrationBuilder.CreateIndex(
                name: "IX_Movies_Name",
                table: "Movies",
                column: "Name",
                unique: true)
                .Annotation("SqlServer:Include", new[] { "Id", "Imbd", "Language", "Description", "DescriptionEs", "Subtitles", "UrlCoverImage", "UrlImage", "UrlVideo", "Year" });

            migrationBuilder.CreateIndex(
                name: "IX_Movies_Recommended",
                table: "Movies",
                column: "Recommended");


        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MoviesWatching_UserId",
                table: "MoviesWatching");

            migrationBuilder.DropIndex(
                name: "IX_Movies_Id",
                table: "Movies");

            migrationBuilder.DropIndex(
                name: "IX_Movies_Imbd",
                table: "Movies");

            migrationBuilder.DropIndex(
                name: "IX_Movies_IsDeleted_ProcessedByStreamVideo",
                table: "Movies");

            migrationBuilder.DropIndex(
                name: "IX_Movies_Language",
                table: "Movies");

            migrationBuilder.DropIndex(
                name: "IX_Movies_Likes",
                table: "Movies");

            migrationBuilder.DropIndex(
                name: "IX_Movies_Name",
                table: "Movies");

            migrationBuilder.DropIndex(
                name: "IX_Movies_Recommended",
                table: "Movies");

            migrationBuilder.DropIndex(
                name: "IX_MovieGenres_MovieId",
                table: "MovieGenres");

            migrationBuilder.DropIndex(
                name: "IX_Genres_Id",
                table: "Genres");

            migrationBuilder.DropColumn(
                name: "ProcessedByStreamVideo",
                table: "Movies");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Movies",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<bool>(
                name: "ProcessedByAzureJob",
                table: "Movies",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Movies_Name_Language_Likes_Recommended_Imbd",
                table: "Movies",
                columns: new[] { "Name", "Language", "Likes", "Recommended", "Imbd" });
        }
    }
}
