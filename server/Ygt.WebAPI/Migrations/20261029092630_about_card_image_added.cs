using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ygt.WebAPI.Migrations
{
    /// <inheritdoc />
    public partial class about_card_image_added : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageFileName",
                table: "AboutCards",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageFileName",
                table: "AboutCards");
        }
    }
}
