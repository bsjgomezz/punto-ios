using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PuntoIOS.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ClientesYLocales : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "locales",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_locales", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "clientes",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    local_id = table.Column<Guid>(type: "uuid", nullable: false),
                    nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    apellido = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    telefono = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    telefono_normalizado = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    email = table.Column<string>(type: "character varying(254)", maxLength: 254, nullable: true),
                    direccion = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    observaciones = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    creado_por_usuario_id = table.Column<Guid>(type: "uuid", nullable: true),
                    creado_en = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_clientes", x => x.id);
                    table.ForeignKey(
                        name: "FK_clientes_locales_local_id",
                        column: x => x.local_id,
                        principalTable: "locales",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "locales",
                columns: new[] { "id", "nombre" },
                values: new object[] { new Guid("0195a000-0000-7000-8000-0000000000a1"), "Punto iOS" });

            migrationBuilder.InsertData(
                table: "permisos",
                columns: new[] { "codigo", "descripcion" },
                values: new object[,]
                {
                    { "clientes.crear", "Registrar clientes nuevos" },
                    { "clientes.ver", "Ver y buscar clientes" }
                });

            migrationBuilder.InsertData(
                table: "roles_permisos",
                columns: new[] { "permiso_codigo", "rol_id" },
                values: new object[,]
                {
                    { "clientes.crear", new Guid("0195a000-0000-7000-8000-000000000001") },
                    { "clientes.ver", new Guid("0195a000-0000-7000-8000-000000000001") },
                    { "clientes.crear", new Guid("0195a000-0000-7000-8000-000000000002") },
                    { "clientes.ver", new Guid("0195a000-0000-7000-8000-000000000002") },
                    { "clientes.ver", new Guid("0195a000-0000-7000-8000-000000000003") }
                });

            migrationBuilder.CreateIndex(
                name: "IX_clientes_local_id_apellido_nombre",
                table: "clientes",
                columns: new[] { "local_id", "apellido", "nombre" });

            migrationBuilder.CreateIndex(
                name: "IX_clientes_local_id_telefono_normalizado",
                table: "clientes",
                columns: new[] { "local_id", "telefono_normalizado" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "clientes");

            migrationBuilder.DropTable(
                name: "locales");

            migrationBuilder.DeleteData(
                table: "roles_permisos",
                keyColumns: new[] { "permiso_codigo", "rol_id" },
                keyValues: new object[] { "clientes.crear", new Guid("0195a000-0000-7000-8000-000000000001") });

            migrationBuilder.DeleteData(
                table: "roles_permisos",
                keyColumns: new[] { "permiso_codigo", "rol_id" },
                keyValues: new object[] { "clientes.ver", new Guid("0195a000-0000-7000-8000-000000000001") });

            migrationBuilder.DeleteData(
                table: "roles_permisos",
                keyColumns: new[] { "permiso_codigo", "rol_id" },
                keyValues: new object[] { "clientes.crear", new Guid("0195a000-0000-7000-8000-000000000002") });

            migrationBuilder.DeleteData(
                table: "roles_permisos",
                keyColumns: new[] { "permiso_codigo", "rol_id" },
                keyValues: new object[] { "clientes.ver", new Guid("0195a000-0000-7000-8000-000000000002") });

            migrationBuilder.DeleteData(
                table: "roles_permisos",
                keyColumns: new[] { "permiso_codigo", "rol_id" },
                keyValues: new object[] { "clientes.ver", new Guid("0195a000-0000-7000-8000-000000000003") });

            migrationBuilder.DeleteData(
                table: "permisos",
                keyColumn: "codigo",
                keyValue: "clientes.crear");

            migrationBuilder.DeleteData(
                table: "permisos",
                keyColumn: "codigo",
                keyValue: "clientes.ver");
        }
    }
}
