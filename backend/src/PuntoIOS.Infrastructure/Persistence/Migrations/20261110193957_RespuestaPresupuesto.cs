using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PuntoIOS.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RespuestaPresupuesto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "respondido_en",
                table: "versiones_presupuesto",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "respondido_por_usuario_id",
                table: "versiones_presupuesto",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "respuesta_aprobada",
                table: "versiones_presupuesto",
                type: "boolean",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "permisos",
                keyColumn: "codigo",
                keyValue: "presupuestos.gestionar",
                column: "descripcion",
                value: "Crear versiones del presupuesto y registrar su aprobación o rechazo (queda registrado)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "respondido_en",
                table: "versiones_presupuesto");

            migrationBuilder.DropColumn(
                name: "respondido_por_usuario_id",
                table: "versiones_presupuesto");

            migrationBuilder.DropColumn(
                name: "respuesta_aprobada",
                table: "versiones_presupuesto");

            migrationBuilder.UpdateData(
                table: "permisos",
                keyColumn: "codigo",
                keyValue: "presupuestos.gestionar",
                column: "descripcion",
                value: "Crear versiones del presupuesto de una orden de reparación (queda registrado)");
        }
    }
}
