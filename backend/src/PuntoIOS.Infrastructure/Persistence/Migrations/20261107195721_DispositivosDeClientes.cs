using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PuntoIOS.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class DispositivosDeClientes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "dispositivos",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    local_id = table.Column<Guid>(type: "uuid", nullable: false),
                    cliente_id = table.Column<Guid>(type: "uuid", nullable: false),
                    marca = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    modelo = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    almacenamiento = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    color = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    imei_serie = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    observaciones = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    creado_por_usuario_id = table.Column<Guid>(type: "uuid", nullable: true),
                    creado_en = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dispositivos", x => x.id);
                    table.ForeignKey(
                        name: "FK_dispositivos_clientes_cliente_id",
                        column: x => x.cliente_id,
                        principalTable: "clientes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_dispositivos_locales_local_id",
                        column: x => x.local_id,
                        principalTable: "locales",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "permisos",
                columns: new[] { "codigo", "descripcion" },
                values: new object[,]
                {
                    { "dispositivos.crear", "Registrar equipos de clientes" },
                    { "dispositivos.editar", "Editar los datos de un equipo" },
                    { "dispositivos.ver", "Ver los equipos de los clientes (IMEI enmascarado)" },
                    { "dispositivos.ver_imei", "Ver el IMEI o número de serie completo (queda registrado)" }
                });

            migrationBuilder.InsertData(
                table: "roles_permisos",
                columns: new[] { "permiso_codigo", "rol_id" },
                values: new object[,]
                {
                    { "dispositivos.crear", new Guid("0195a000-0000-7000-8000-000000000001") },
                    { "dispositivos.editar", new Guid("0195a000-0000-7000-8000-000000000001") },
                    { "dispositivos.ver", new Guid("0195a000-0000-7000-8000-000000000001") },
                    { "dispositivos.ver_imei", new Guid("0195a000-0000-7000-8000-000000000001") },
                    { "dispositivos.crear", new Guid("0195a000-0000-7000-8000-000000000002") },
                    { "dispositivos.editar", new Guid("0195a000-0000-7000-8000-000000000002") },
                    { "dispositivos.ver", new Guid("0195a000-0000-7000-8000-000000000002") },
                    { "dispositivos.ver_imei", new Guid("0195a000-0000-7000-8000-000000000002") },
                    { "dispositivos.ver", new Guid("0195a000-0000-7000-8000-000000000003") },
                    { "dispositivos.ver_imei", new Guid("0195a000-0000-7000-8000-000000000003") }
                });

            migrationBuilder.CreateIndex(
                name: "IX_dispositivos_cliente_id",
                table: "dispositivos",
                column: "cliente_id");

            migrationBuilder.CreateIndex(
                name: "IX_dispositivos_local_id_cliente_id",
                table: "dispositivos",
                columns: new[] { "local_id", "cliente_id" });

            migrationBuilder.CreateIndex(
                name: "IX_dispositivos_local_id_imei_serie",
                table: "dispositivos",
                columns: new[] { "local_id", "imei_serie" },
                unique: true,
                filter: "imei_serie IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "dispositivos");

            migrationBuilder.DeleteData(
                table: "roles_permisos",
                keyColumns: new[] { "permiso_codigo", "rol_id" },
                keyValues: new object[] { "dispositivos.crear", new Guid("0195a000-0000-7000-8000-000000000001") });

            migrationBuilder.DeleteData(
                table: "roles_permisos",
                keyColumns: new[] { "permiso_codigo", "rol_id" },
                keyValues: new object[] { "dispositivos.editar", new Guid("0195a000-0000-7000-8000-000000000001") });

            migrationBuilder.DeleteData(
                table: "roles_permisos",
                keyColumns: new[] { "permiso_codigo", "rol_id" },
                keyValues: new object[] { "dispositivos.ver", new Guid("0195a000-0000-7000-8000-000000000001") });

            migrationBuilder.DeleteData(
                table: "roles_permisos",
                keyColumns: new[] { "permiso_codigo", "rol_id" },
                keyValues: new object[] { "dispositivos.ver_imei", new Guid("0195a000-0000-7000-8000-000000000001") });

            migrationBuilder.DeleteData(
                table: "roles_permisos",
                keyColumns: new[] { "permiso_codigo", "rol_id" },
                keyValues: new object[] { "dispositivos.crear", new Guid("0195a000-0000-7000-8000-000000000002") });

            migrationBuilder.DeleteData(
                table: "roles_permisos",
                keyColumns: new[] { "permiso_codigo", "rol_id" },
                keyValues: new object[] { "dispositivos.editar", new Guid("0195a000-0000-7000-8000-000000000002") });

            migrationBuilder.DeleteData(
                table: "roles_permisos",
                keyColumns: new[] { "permiso_codigo", "rol_id" },
                keyValues: new object[] { "dispositivos.ver", new Guid("0195a000-0000-7000-8000-000000000002") });

            migrationBuilder.DeleteData(
                table: "roles_permisos",
                keyColumns: new[] { "permiso_codigo", "rol_id" },
                keyValues: new object[] { "dispositivos.ver_imei", new Guid("0195a000-0000-7000-8000-000000000002") });

            migrationBuilder.DeleteData(
                table: "roles_permisos",
                keyColumns: new[] { "permiso_codigo", "rol_id" },
                keyValues: new object[] { "dispositivos.ver", new Guid("0195a000-0000-7000-8000-000000000003") });

            migrationBuilder.DeleteData(
                table: "roles_permisos",
                keyColumns: new[] { "permiso_codigo", "rol_id" },
                keyValues: new object[] { "dispositivos.ver_imei", new Guid("0195a000-0000-7000-8000-000000000003") });

            migrationBuilder.DeleteData(
                table: "permisos",
                keyColumn: "codigo",
                keyValue: "dispositivos.crear");

            migrationBuilder.DeleteData(
                table: "permisos",
                keyColumn: "codigo",
                keyValue: "dispositivos.editar");

            migrationBuilder.DeleteData(
                table: "permisos",
                keyColumn: "codigo",
                keyValue: "dispositivos.ver");

            migrationBuilder.DeleteData(
                table: "permisos",
                keyColumn: "codigo",
                keyValue: "dispositivos.ver_imei");
        }
    }
}
