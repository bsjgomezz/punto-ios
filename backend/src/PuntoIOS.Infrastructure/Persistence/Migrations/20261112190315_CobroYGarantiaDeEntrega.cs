using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PuntoIOS.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class CobroYGarantiaDeEntrega : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "entregado_en",
                table: "ordenes_reparacion",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "garantia_codigo",
                table: "ordenes_reparacion",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "garantia_dias",
                table: "ordenes_reparacion",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "garantia_vence_en",
                table: "ordenes_reparacion",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "metodo_pago",
                table: "ordenes_reparacion",
                type: "character varying(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "monto_cobrado",
                table: "ordenes_reparacion",
                type: "numeric(12,2)",
                precision: 12,
                scale: 2,
                nullable: true);

            migrationBuilder.InsertData(
                table: "permisos",
                columns: new[] { "codigo", "descripcion" },
                values: new object[] { "reparaciones.entregar", "Entregar el equipo, registrar el cobro y la garantía (queda registrado)" });

            migrationBuilder.InsertData(
                table: "roles_permisos",
                columns: new[] { "permiso_codigo", "rol_id" },
                values: new object[,]
                {
                    { "reparaciones.entregar", new Guid("0195a000-0000-7000-8000-000000000001") },
                    { "reparaciones.entregar", new Guid("0195a000-0000-7000-8000-000000000002") }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "roles_permisos",
                keyColumns: new[] { "permiso_codigo", "rol_id" },
                keyValues: new object[] { "reparaciones.entregar", new Guid("0195a000-0000-7000-8000-000000000001") });

            migrationBuilder.DeleteData(
                table: "roles_permisos",
                keyColumns: new[] { "permiso_codigo", "rol_id" },
                keyValues: new object[] { "reparaciones.entregar", new Guid("0195a000-0000-7000-8000-000000000002") });

            migrationBuilder.DeleteData(
                table: "permisos",
                keyColumn: "codigo",
                keyValue: "reparaciones.entregar");

            migrationBuilder.DropColumn(
                name: "entregado_en",
                table: "ordenes_reparacion");

            migrationBuilder.DropColumn(
                name: "garantia_codigo",
                table: "ordenes_reparacion");

            migrationBuilder.DropColumn(
                name: "garantia_dias",
                table: "ordenes_reparacion");

            migrationBuilder.DropColumn(
                name: "garantia_vence_en",
                table: "ordenes_reparacion");

            migrationBuilder.DropColumn(
                name: "metodo_pago",
                table: "ordenes_reparacion");

            migrationBuilder.DropColumn(
                name: "monto_cobrado",
                table: "ordenes_reparacion");
        }
    }
}
