using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PuntoIOS.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class PermisoCambiarEstado : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "permisos",
                columns: new[] { "codigo", "descripcion" },
                values: new object[] { "reparaciones.cambiar_estado", "Cambiar el estado de una orden de reparación (queda registrado)" });

            migrationBuilder.InsertData(
                table: "roles_permisos",
                columns: new[] { "permiso_codigo", "rol_id" },
                values: new object[,]
                {
                    { "reparaciones.cambiar_estado", new Guid("0195a000-0000-7000-8000-000000000001") },
                    { "reparaciones.cambiar_estado", new Guid("0195a000-0000-7000-8000-000000000002") },
                    { "reparaciones.cambiar_estado", new Guid("0195a000-0000-7000-8000-000000000003") }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "roles_permisos",
                keyColumns: new[] { "permiso_codigo", "rol_id" },
                keyValues: new object[] { "reparaciones.cambiar_estado", new Guid("0195a000-0000-7000-8000-000000000001") });

            migrationBuilder.DeleteData(
                table: "roles_permisos",
                keyColumns: new[] { "permiso_codigo", "rol_id" },
                keyValues: new object[] { "reparaciones.cambiar_estado", new Guid("0195a000-0000-7000-8000-000000000002") });

            migrationBuilder.DeleteData(
                table: "roles_permisos",
                keyColumns: new[] { "permiso_codigo", "rol_id" },
                keyValues: new object[] { "reparaciones.cambiar_estado", new Guid("0195a000-0000-7000-8000-000000000003") });

            migrationBuilder.DeleteData(
                table: "permisos",
                keyColumn: "codigo",
                keyValue: "reparaciones.cambiar_estado");
        }
    }
}
