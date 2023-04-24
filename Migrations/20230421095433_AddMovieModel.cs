using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace movie_tracker_website.Migrations
{
    /// <inheritdoc />
    public partial class AddMovieModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Movies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApiId = table.Column<int>(type: "int", nullable: false),
                    Rating = table.Column<double>(type: "float", nullable: true),
                    AppUserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    AppUserId1 = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    AppUserId2 = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Movies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Movies_AspNetUsers_AppUserId",
                        column: x => x.AppUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Movies_AspNetUsers_AppUserId1",
                        column: x => x.AppUserId1,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Movies_AspNetUsers_AppUserId2",
                        column: x => x.AppUserId2,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Movies_AppUserId",
                table: "Movies",
                column: "AppUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Movies_AppUserId1",
                table: "Movies",
                column: "AppUserId1");

            migrationBuilder.CreateIndex(
                name: "IX_Movies_AppUserId2",
                table: "Movies",
                column: "AppUserId2");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Movies");
        }
    }
}
