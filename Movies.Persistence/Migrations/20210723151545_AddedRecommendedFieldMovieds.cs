using Microsoft.EntityFrameworkCore.Migrations;

namespace Movies.Persistence.Migrations
{
    public partial class AddedRecommendedFieldMovieds : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Movies_Name_Gender_Language",
                table: "Movies");

            migrationBuilder.AddColumn<bool>(
                name: "Recommended",
                table: "Movies",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Movies_Name_Gender_Language_Likes_Recommended",
                table: "Movies",
                columns: new[] { "Name", "Gender", "Language", "Likes", "Recommended" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Movies_Name_Gender_Language_Likes_Recommended",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "Recommended",
                table: "Movies");

            migrationBuilder.CreateIndex(
                name: "IX_Movies_Name_Gender_Language",
                table: "Movies",
                columns: new[] { "Name", "Gender", "Language" });
        }
    }
}
