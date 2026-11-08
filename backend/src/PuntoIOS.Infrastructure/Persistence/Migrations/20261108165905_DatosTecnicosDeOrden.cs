using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PuntoIOS.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class DatosTecnicosDeOrden : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<List<string>>(
                name: "componentes_afectados",
                table: "ordenes_reparacion",
                type: "text[]",
                nullable: false,
                defaultValueSql: "'{}'");

            migrationBuilder.AddColumn<string>(
                name: "diagnostico",
                table: "ordenes_reparacion",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "mediciones",
                table: "ordenes_reparacion",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "notas_internas",
                table: "ordenes_reparacion",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "pruebas",
                table: "ordenes_reparacion",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "componentes_afectados",
                table: "ordenes_reparacion");

            migrationBuilder.DropColumn(
                name: "diagnostico",
                table: "ordenes_reparacion");

            migrationBuilder.DropColumn(
                name: "mediciones",
                table: "ordenes_reparacion");

            migrationBuilder.DropColumn(
                name: "notas_internas",
                table: "ordenes_reparacion");

            migrationBuilder.DropColumn(
                name: "pruebas",
                table: "ordenes_reparacion");
        }
    }
}
