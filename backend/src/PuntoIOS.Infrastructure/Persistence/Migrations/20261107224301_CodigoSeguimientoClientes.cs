using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PuntoIOS.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class CodigoSeguimientoClientes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "codigo_seguimiento_cifrado",
                table: "clientes",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "codigo_seguimiento_generado_en",
                table: "clientes",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "codigo_seguimiento_huella",
                table: "clientes",
                type: "character varying(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.InsertData(
                table: "permisos",
                columns: new[] { "codigo", "descripcion" },
                values: new object[] { "clientes.seguimiento", "Ver, enviar y regenerar el código de seguimiento del cliente (queda registrado)" });

            migrationBuilder.InsertData(
                table: "roles_permisos",
                columns: new[] { "permiso_codigo", "rol_id" },
                values: new object[,]
                {
                    { "clientes.seguimiento", new Guid("0195a000-0000-7000-8000-000000000001") },
                    { "clientes.seguimiento", new Guid("0195a000-0000-7000-8000-000000000002") }
                });

            migrationBuilder.CreateIndex(
                name: "IX_clientes_codigo_seguimiento_huella",
                table: "clientes",
                column: "codigo_seguimiento_huella",
                unique: true,
                filter: "codigo_seguimiento_huella IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_clientes_codigo_seguimiento_huella",
                table: "clientes");

            migrationBuilder.DeleteData(
                table: "roles_permisos",
                keyColumns: new[] { "permiso_codigo", "rol_id" },
                keyValues: new object[] { "clientes.seguimiento", new Guid("0195a000-0000-7000-8000-000000000001") });

            migrationBuilder.DeleteData(
                table: "roles_permisos",
                keyColumns: new[] { "permiso_codigo", "rol_id" },
                keyValues: new object[] { "clientes.seguimiento", new Guid("0195a000-0000-7000-8000-000000000002") });

            migrationBuilder.DeleteData(
                table: "permisos",
                keyColumn: "codigo",
                keyValue: "clientes.seguimiento");

            migrationBuilder.DropColumn(
                name: "codigo_seguimiento_cifrado",
                table: "clientes");

            migrationBuilder.DropColumn(
                name: "codigo_seguimiento_generado_en",
                table: "clientes");

            migrationBuilder.DropColumn(
                name: "codigo_seguimiento_huella",
                table: "clientes");
        }
    }
}
