using Microsoft.EntityFrameworkCore.Migrations;

namespace Movies.Persistence.Migrations
{
    public partial class AddMovieAssetAndDirector : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FileName",
                table: "Movies",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "MovieAzureAsset",
                columns: table => new
                {
                    MovieId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MovieId1 = table.Column<int>(type: "int", nullable: false),
                    AssetInputName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AssetOutput = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ProcessedByAzureJob = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovieAzureAsset", x => x.MovieId)
                        .Annotation("SqlServer:Clustered", true);
                    table.ForeignKey(
                        name: "FK_MovieAzureAsset_Movies_MovieId1",
                        column: x => x.MovieId1,
                        principalTable: "Movies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MovieAzureAsset_AssetOutput",
                table: "MovieAzureAsset",
                column: "AssetOutput");

            migrationBuilder.CreateIndex(
                name: "IX_MovieAzureAsset_MovieId1",
                table: "MovieAzureAsset",
                column: "MovieId1");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MovieAzureAsset");

            migrationBuilder.DropColumn(
                name: "FileName",
                table: "Movies");
        }
    }
}
