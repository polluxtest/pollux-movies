using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Movies.Persistence.Migrations
{
    public partial class AddMovie : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MovieWatching_Movies_MovieId",
                table: "MoviesWatching");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MovieWatching",
                table: "MoviesWatching");

            migrationBuilder.RenameIndex(
                name: "IX_MovieWatching_UserId_MovieId",
                table: "MoviesWatching",
                newName: "IX_MoviesWatching_UserId_MovieId");

            migrationBuilder.RenameIndex(
                name: "IX_MovieWatching_MovieId",
                table: "MoviesWatching",
                newName: "IX_MoviesWatching_MovieId");

            migrationBuilder.AlterColumn<decimal>(
                name: "ElapsedTime",
                table: "MoviesWatching",
                type: "decimal",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldDefaultValue: 0m);

            migrationBuilder.AlterColumn<decimal>(
                name: "Duration",
                table: "MoviesWatching",
                type: "decimal",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldDefaultValue: 0m);

            migrationBuilder.AddPrimaryKey(
                name: "PK_MoviesWatching",
                table: "MoviesWatching",
                columns: new[] { "UserId", "MovieId" });

            migrationBuilder.AddForeignKey(
                name: "FK_MoviesWatching_Movies_MovieId",
                table: "MoviesWatching",
                column: "MovieId",
                principalTable: "Movies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MoviesWatching_Movies_MovieId",
                table: "MoviesWatching");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MoviesWatching",
                table: "MoviesWatching");


            migrationBuilder.RenameIndex(
                name: "IX_MoviesWatching_UserId_MovieId",
                table: "MoviesWatching",
                newName: "IX_MovieWatching_UserId_MovieId");

            migrationBuilder.RenameIndex(
                name: "IX_MoviesWatching_MovieId",
                table: "MoviesWatching",
                newName: "IX_MovieWatching_MovieId");

            migrationBuilder.AlterColumn<decimal>(
                name: "ElapsedTime",
                table: "MoviesWatching",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal",
                oldDefaultValue: 0m);

            migrationBuilder.AlterColumn<decimal>(
                name: "Duration",
                table: "MoviesWatching",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal",
                oldDefaultValue: 0m);

            migrationBuilder.AddPrimaryKey(
                name: "PK_MovieWatching",
                table: "MoviesWatching",
                columns: new[] { "UserId", "MovieId" });

            migrationBuilder.AddForeignKey(
                name: "FK_MovieWatching_Movies_MovieId",
                table: "MoviesWatching",
                column: "MovieId",
                principalTable: "Movies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
