using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PuntoIOS.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class VersionesPresupuesto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "versiones_presupuesto",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    local_id = table.Column<Guid>(type: "uuid", nullable: false),
                    orden_id = table.Column<Guid>(type: "uuid", nullable: false),
                    numero = table.Column<int>(type: "integer", nullable: false),
                    total = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    creado_por_usuario_id = table.Column<Guid>(type: "uuid", nullable: true),
                    creado_en = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    items = table.Column<string>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_versiones_presupuesto", x => x.id);
                    table.ForeignKey(
                        name: "FK_versiones_presupuesto_locales_local_id",
                        column: x => x.local_id,
                        principalTable: "locales",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_versiones_presupuesto_ordenes_reparacion_orden_id",
                        column: x => x.orden_id,
                        principalTable: "ordenes_reparacion",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "permisos",
                columns: new[] { "codigo", "descripcion" },
                values: new object[] { "presupuestos.gestionar", "Crear versiones del presupuesto de una orden de reparación (queda registrado)" });

            migrationBuilder.InsertData(
                table: "roles_permisos",
                columns: new[] { "permiso_codigo", "rol_id" },
                values: new object[,]
                {
                    { "presupuestos.gestionar", new Guid("0195a000-0000-7000-8000-000000000001") },
                    { "presupuestos.gestionar", new Guid("0195a000-0000-7000-8000-000000000002") },
                    { "presupuestos.gestionar", new Guid("0195a000-0000-7000-8000-000000000003") }
                });

            migrationBuilder.CreateIndex(
                name: "IX_versiones_presupuesto_local_id",
                table: "versiones_presupuesto",
                column: "local_id");

            migrationBuilder.CreateIndex(
                name: "IX_versiones_presupuesto_orden_id_numero",
                table: "versiones_presupuesto",
                columns: new[] { "orden_id", "numero" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "versiones_presupuesto");

            migrationBuilder.DeleteData(
                table: "roles_permisos",
                keyColumns: new[] { "permiso_codigo", "rol_id" },
                keyValues: new object[] { "presupuestos.gestionar", new Guid("0195a000-0000-7000-8000-000000000001") });

            migrationBuilder.DeleteData(
                table: "roles_permisos",
                keyColumns: new[] { "permiso_codigo", "rol_id" },
                keyValues: new object[] { "presupuestos.gestionar", new Guid("0195a000-0000-7000-8000-000000000002") });

            migrationBuilder.DeleteData(
                table: "roles_permisos",
                keyColumns: new[] { "permiso_codigo", "rol_id" },
                keyValues: new object[] { "presupuestos.gestionar", new Guid("0195a000-0000-7000-8000-000000000003") });

            migrationBuilder.DeleteData(
                table: "permisos",
                keyColumn: "codigo",
                keyValue: "presupuestos.gestionar");
        }
    }
}
