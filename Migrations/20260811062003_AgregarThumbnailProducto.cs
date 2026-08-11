using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MacrobioticaLaBendicion.Migrations
{
    /// <inheritdoc />
    public partial class AgregarThumbnailProducto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Url_Thumbnail",
                table: "Productos",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Url_Thumbnail",
                table: "Productos");
        }
    }
}
