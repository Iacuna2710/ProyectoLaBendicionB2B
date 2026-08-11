using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MacrobioticaLaBendicion.Migrations
{
    /// <inheritdoc />
    public partial class AgregarBitacora : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Bitacoras",
                columns: table => new
                {
                    id_Bitacora = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    UsuarioNombre = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Accion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Entidad = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EntidadId = table.Column<int>(type: "int", nullable: true),
                    Detalle = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Fecha = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bitacoras", x => x.id_Bitacora);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bitacoras_Entidad",
                table: "Bitacoras",
                column: "Entidad");

            migrationBuilder.CreateIndex(
                name: "IX_Bitacoras_Fecha",
                table: "Bitacoras",
                column: "Fecha");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Bitacoras");
        }
    }
}
