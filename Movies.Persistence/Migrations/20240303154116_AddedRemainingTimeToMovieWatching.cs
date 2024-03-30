using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Movies.Persistence.Migrations
{
    public partial class AddedRemainingTimeToMovieWatching : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "ElapsedTime",
                table: "MoviesWatching",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(decimal),
                oldType: "decimal",
                oldDefaultValue: 0m);

            migrationBuilder.AlterColumn<int>(
                name: "Duration",
                table: "MoviesWatching",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(decimal),
                oldType: "decimal",
                oldDefaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "RemainingTime",
                table: "MoviesWatching",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RemainingTime",
                table: "MoviesWatching");

            migrationBuilder.AlterColumn<decimal>(
                name: "ElapsedTime",
                table: "MoviesWatching",
                type: "decimal",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 0);

            migrationBuilder.AlterColumn<decimal>(
                name: "Duration",
                table: "MoviesWatching",
                type: "decimal",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 0);
        }
    }
}
