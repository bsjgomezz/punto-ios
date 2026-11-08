using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PuntoIOS.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ChecklistYFotosDeOrdenes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "checklist_ingreso",
                table: "ordenes_reparacion",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "notas_checklist",
                table: "ordenes_reparacion",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "salud_bateria",
                table: "ordenes_reparacion",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "fotos_orden",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    local_id = table.Column<Guid>(type: "uuid", nullable: false),
                    orden_id = table.Column<Guid>(type: "uuid", nullable: false),
                    ruta = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    tipo_contenido = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    tamano = table.Column<long>(type: "bigint", nullable: false),
                    creado_por_usuario_id = table.Column<Guid>(type: "uuid", nullable: true),
                    creado_en = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_fotos_orden", x => x.id);
                    table.ForeignKey(
                        name: "FK_fotos_orden_locales_local_id",
                        column: x => x.local_id,
                        principalTable: "locales",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_fotos_orden_ordenes_reparacion_orden_id",
                        column: x => x.orden_id,
                        principalTable: "ordenes_reparacion",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_fotos_orden_local_id_orden_id_creado_en",
                table: "fotos_orden",
                columns: new[] { "local_id", "orden_id", "creado_en" });

            migrationBuilder.CreateIndex(
                name: "IX_fotos_orden_orden_id",
                table: "fotos_orden",
                column: "orden_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "fotos_orden");

            migrationBuilder.DropColumn(
                name: "checklist_ingreso",
                table: "ordenes_reparacion");

            migrationBuilder.DropColumn(
                name: "notas_checklist",
                table: "ordenes_reparacion");

            migrationBuilder.DropColumn(
                name: "salud_bateria",
                table: "ordenes_reparacion");
        }
    }
}
