using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace Movies.Persistence.Migrations
{
    public partial class DropAndRecreateTablesWithGuid : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.CreateTable(
                name: "Movies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Language = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Year = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Likes = table.Column<int>(type: "int", maxLength: 100, nullable: false),
                    UrlVideo = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    UrlImage = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    UrlCoverImage = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Subtitles = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Recommended = table.Column<bool>(type: "bit", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    ProcessedByAzureJob = table.Column<bool>(type: "bit", nullable: false),
                    DirectorId = table.Column<bool>(type: "int", nullable: false, defaultValue: 1),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Movies", x => x.Id);
                });


            migrationBuilder.CreateIndex(
                name: "IX_Movies_Name_Gender_Language",
                table: "Movies",
                columns: new[] { "Name", "Gender", "Language" });


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
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserMovies_UserId_MovieId",
                table: "UserMovies",
                columns: new[] { "UserId", "MovieId" });


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
                        onDelete: ReferentialAction.NoAction);
                });

            // user movies likes

            migrationBuilder.CreateIndex(
                name: "IX_UserMovies_MovieId",
                table: "UserMovies",
                column: "MovieId");



            // user likes indexes

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

            // director index

            migrationBuilder.CreateIndex(
                name: "IX_Movies_DirectorId",
                table: "Movies",
                column: "DirectorId");

            // recommended index

            migrationBuilder.CreateIndex(
                name: "IX_Movies_Name_Gender_Language_Likes_Recommended",
                table: "Movies",
                columns: new[] { "Name", "Gender", "Language", "Likes", "Recommended" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserMovies");

            migrationBuilder.DropTable(
                name: "UserLikes");

            migrationBuilder.DropTable(
                name: "Movies");
        }
    }
}
