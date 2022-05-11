using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Neverland.Data.Migrations
{
    public partial class modifymoviescore2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MovieScore_Movies_MovieId",
                table: "MovieScore");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MovieScore",
                table: "MovieScore");

            migrationBuilder.RenameTable(
                name: "MovieScore",
                newName: "MovieScores");

            migrationBuilder.RenameIndex(
                name: "IX_MovieScore_MovieId",
                table: "MovieScores",
                newName: "IX_MovieScores_MovieId");

            migrationBuilder.AddColumn<int>(
                name: "PaymentType",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_MovieScores",
                table: "MovieScores",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MovieScores_Movies_MovieId",
                table: "MovieScores",
                column: "MovieId",
                principalTable: "Movies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MovieScores_Movies_MovieId",
                table: "MovieScores");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MovieScores",
                table: "MovieScores");

            migrationBuilder.DropColumn(
                name: "PaymentType",
                table: "Orders");

            migrationBuilder.RenameTable(
                name: "MovieScores",
                newName: "MovieScore");

            migrationBuilder.RenameIndex(
                name: "IX_MovieScores_MovieId",
                table: "MovieScore",
                newName: "IX_MovieScore_MovieId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MovieScore",
                table: "MovieScore",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MovieScore_Movies_MovieId",
                table: "MovieScore",
                column: "MovieId",
                principalTable: "Movies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
