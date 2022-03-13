using Microsoft.EntityFrameworkCore.Migrations;

namespace Movies.Persistence.Migrations
{
    public partial class AddedMovieImbdScoreField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Movies_Name_Gender_Language_Likes_Recommended",
                table: "Movies");

            migrationBuilder.AddColumn<string>(
                name: "Imbd",
                table: "Movies",
                type: "nvarchar(5)",
                maxLength: 5,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Movies_Name_Gender_Language_Likes_Recommended_Imbd",
                table: "Movies",
                columns: new[] { "Name", "Gender", "Language", "Likes", "Recommended", "Imbd" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Movies_Name_Gender_Language_Likes_Recommended_Imbd",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "Imbd",
                table: "Movies");

            migrationBuilder.CreateIndex(
                name: "IX_Movies_Name_Gender_Language_Likes_Recommended",
                table: "Movies",
                columns: new[] { "Name", "Gender", "Language", "Likes", "Recommended" });
        }
    }
}
