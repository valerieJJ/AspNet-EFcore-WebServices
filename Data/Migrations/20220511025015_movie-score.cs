using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Neverland.Data.Migrations
{
    public partial class moviescore : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Score",
                table: "MovieDetails",
                type: "double",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Score",
                table: "MovieDetails");
        }
    }
}
