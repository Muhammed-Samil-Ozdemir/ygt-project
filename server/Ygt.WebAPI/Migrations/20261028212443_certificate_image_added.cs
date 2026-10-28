using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ygt.WebAPI.Migrations
{
    /// <inheritdoc />
    public partial class certificate_image_added : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageFileName",
                table: "Certificates",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageFileName",
                table: "Certificates");
        }
    }
}
