using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace movie_tracker_website.Migrations
{
    /// <inheritdoc />
    public partial class fixedBoolName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IfMarked",
                table: "Movies",
                newName: "IfToWatch");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IfToWatch",
                table: "Movies",
                newName: "IfMarked");
        }
    }
}
