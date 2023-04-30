using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace movie_tracker_website.Migrations
{
    /// <inheritdoc />
    public partial class reworkedMoviesLists : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Movies_AspNetUsers_AppUserId1",
                table: "Movies");

            migrationBuilder.DropForeignKey(
                name: "FK_Movies_AspNetUsers_AppUserId2",
                table: "Movies");

            migrationBuilder.DropIndex(
                name: "IX_Movies_AppUserId1",
                table: "Movies");

            migrationBuilder.DropIndex(
                name: "IX_Movies_AppUserId2",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "AppUserId1",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "AppUserId2",
                table: "Movies");

            migrationBuilder.AddColumn<bool>(
                name: "IfFavourite",
                table: "Movies",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IfMarked",
                table: "Movies",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IfWatched",
                table: "Movies",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IfFavourite",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "IfMarked",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "IfWatched",
                table: "Movies");

            migrationBuilder.AddColumn<string>(
                name: "AppUserId1",
                table: "Movies",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AppUserId2",
                table: "Movies",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Movies_AppUserId1",
                table: "Movies",
                column: "AppUserId1");

            migrationBuilder.CreateIndex(
                name: "IX_Movies_AppUserId2",
                table: "Movies",
                column: "AppUserId2");

            migrationBuilder.AddForeignKey(
                name: "FK_Movies_AspNetUsers_AppUserId1",
                table: "Movies",
                column: "AppUserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Movies_AspNetUsers_AppUserId2",
                table: "Movies",
                column: "AppUserId2",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
