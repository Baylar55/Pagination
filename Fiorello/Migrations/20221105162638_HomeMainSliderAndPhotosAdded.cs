using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fiorello.Migrations
{
    public partial class HomeMainSliderAndPhotosAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HomeMainSlider",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SubPhotoName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HomeMainSlider", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HomeMainSliderPhotos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    HomeMainSliderId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HomeMainSliderPhotos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HomeMainSliderPhotos_HomeMainSlider_HomeMainSliderId",
                        column: x => x.HomeMainSliderId,
                        principalTable: "HomeMainSlider",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HomeMainSliderPhotos_HomeMainSliderId",
                table: "HomeMainSliderPhotos",
                column: "HomeMainSliderId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HomeMainSliderPhotos");

            migrationBuilder.DropTable(
                name: "HomeMainSlider");
        }
    }
}
