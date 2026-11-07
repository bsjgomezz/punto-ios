using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PuntoIOS.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class OrdenesDeReparacion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence(
                name: "ordenes_reparacion_numero_seq");

            migrationBuilder.CreateTable(
                name: "ordenes_reparacion",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    local_id = table.Column<Guid>(type: "uuid", nullable: false),
                    numero = table.Column<long>(type: "bigint", nullable: false, defaultValueSql: "nextval('ordenes_reparacion_numero_seq')"),
                    cliente_id = table.Column<Guid>(type: "uuid", nullable: false),
                    dispositivo_id = table.Column<Guid>(type: "uuid", nullable: false),
                    problema_informado = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    tipos_reparacion = table.Column<List<string>>(type: "text[]", nullable: false),
                    otro_tipo_descripcion = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    tecnico_usuario_id = table.Column<Guid>(type: "uuid", nullable: true),
                    fecha_estimada_entrega = table.Column<DateOnly>(type: "date", nullable: true),
                    estado = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    desbloqueo_tipo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    desbloqueo_cifrado = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    creado_por_usuario_id = table.Column<Guid>(type: "uuid", nullable: true),
                    creado_en = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ordenes_reparacion", x => x.id);
                    table.ForeignKey(
                        name: "FK_ordenes_reparacion_clientes_cliente_id",
                        column: x => x.cliente_id,
                        principalTable: "clientes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ordenes_reparacion_dispositivos_dispositivo_id",
                        column: x => x.dispositivo_id,
                        principalTable: "dispositivos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ordenes_reparacion_locales_local_id",
                        column: x => x.local_id,
                        principalTable: "locales",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ordenes_reparacion_usuarios_tecnico_usuario_id",
                        column: x => x.tecnico_usuario_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "permisos",
                columns: new[] { "codigo", "descripcion" },
                values: new object[,]
                {
                    { "reparaciones.crear", "Recibir equipos y crear órdenes de reparación" },
                    { "reparaciones.reparar", "Trabajar en reparaciones (se le pueden asignar órdenes)" },
                    { "reparaciones.ver", "Ver las órdenes de reparación" },
                    { "reparaciones.ver_desbloqueo", "Ver el código o patrón de desbloqueo del equipo (queda registrado)" }
                });

            migrationBuilder.InsertData(
                table: "roles_permisos",
                columns: new[] { "permiso_codigo", "rol_id" },
                values: new object[,]
                {
                    { "reparaciones.crear", new Guid("0195a000-0000-7000-8000-000000000001") },
                    { "reparaciones.reparar", new Guid("0195a000-0000-7000-8000-000000000001") },
                    { "reparaciones.ver", new Guid("0195a000-0000-7000-8000-000000000001") },
                    { "reparaciones.ver_desbloqueo", new Guid("0195a000-0000-7000-8000-000000000001") },
                    { "reparaciones.crear", new Guid("0195a000-0000-7000-8000-000000000002") },
                    { "reparaciones.ver", new Guid("0195a000-0000-7000-8000-000000000002") },
                    { "reparaciones.reparar", new Guid("0195a000-0000-7000-8000-000000000003") },
                    { "reparaciones.ver", new Guid("0195a000-0000-7000-8000-000000000003") },
                    { "reparaciones.ver_desbloqueo", new Guid("0195a000-0000-7000-8000-000000000003") }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ordenes_reparacion_cliente_id",
                table: "ordenes_reparacion",
                column: "cliente_id");

            migrationBuilder.CreateIndex(
                name: "IX_ordenes_reparacion_dispositivo_id",
                table: "ordenes_reparacion",
                column: "dispositivo_id");

            migrationBuilder.CreateIndex(
                name: "IX_ordenes_reparacion_local_id_cliente_id",
                table: "ordenes_reparacion",
                columns: new[] { "local_id", "cliente_id" });

            migrationBuilder.CreateIndex(
                name: "IX_ordenes_reparacion_local_id_creado_en",
                table: "ordenes_reparacion",
                columns: new[] { "local_id", "creado_en" });

            migrationBuilder.CreateIndex(
                name: "IX_ordenes_reparacion_local_id_tecnico_usuario_id_estado",
                table: "ordenes_reparacion",
                columns: new[] { "local_id", "tecnico_usuario_id", "estado" });

            migrationBuilder.CreateIndex(
                name: "IX_ordenes_reparacion_numero",
                table: "ordenes_reparacion",
                column: "numero",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ordenes_reparacion_tecnico_usuario_id",
                table: "ordenes_reparacion",
                column: "tecnico_usuario_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ordenes_reparacion");

            migrationBuilder.DeleteData(
                table: "roles_permisos",
                keyColumns: new[] { "permiso_codigo", "rol_id" },
                keyValues: new object[] { "reparaciones.crear", new Guid("0195a000-0000-7000-8000-000000000001") });

            migrationBuilder.DeleteData(
                table: "roles_permisos",
                keyColumns: new[] { "permiso_codigo", "rol_id" },
                keyValues: new object[] { "reparaciones.reparar", new Guid("0195a000-0000-7000-8000-000000000001") });

            migrationBuilder.DeleteData(
                table: "roles_permisos",
                keyColumns: new[] { "permiso_codigo", "rol_id" },
                keyValues: new object[] { "reparaciones.ver", new Guid("0195a000-0000-7000-8000-000000000001") });

            migrationBuilder.DeleteData(
                table: "roles_permisos",
                keyColumns: new[] { "permiso_codigo", "rol_id" },
                keyValues: new object[] { "reparaciones.ver_desbloqueo", new Guid("0195a000-0000-7000-8000-000000000001") });

            migrationBuilder.DeleteData(
                table: "roles_permisos",
                keyColumns: new[] { "permiso_codigo", "rol_id" },
                keyValues: new object[] { "reparaciones.crear", new Guid("0195a000-0000-7000-8000-000000000002") });

            migrationBuilder.DeleteData(
                table: "roles_permisos",
                keyColumns: new[] { "permiso_codigo", "rol_id" },
                keyValues: new object[] { "reparaciones.ver", new Guid("0195a000-0000-7000-8000-000000000002") });

            migrationBuilder.DeleteData(
                table: "roles_permisos",
                keyColumns: new[] { "permiso_codigo", "rol_id" },
                keyValues: new object[] { "reparaciones.reparar", new Guid("0195a000-0000-7000-8000-000000000003") });

            migrationBuilder.DeleteData(
                table: "roles_permisos",
                keyColumns: new[] { "permiso_codigo", "rol_id" },
                keyValues: new object[] { "reparaciones.ver", new Guid("0195a000-0000-7000-8000-000000000003") });

            migrationBuilder.DeleteData(
                table: "roles_permisos",
                keyColumns: new[] { "permiso_codigo", "rol_id" },
                keyValues: new object[] { "reparaciones.ver_desbloqueo", new Guid("0195a000-0000-7000-8000-000000000003") });

            migrationBuilder.DeleteData(
                table: "permisos",
                keyColumn: "codigo",
                keyValue: "reparaciones.crear");

            migrationBuilder.DeleteData(
                table: "permisos",
                keyColumn: "codigo",
                keyValue: "reparaciones.reparar");

            migrationBuilder.DeleteData(
                table: "permisos",
                keyColumn: "codigo",
                keyValue: "reparaciones.ver");

            migrationBuilder.DeleteData(
                table: "permisos",
                keyColumn: "codigo",
                keyValue: "reparaciones.ver_desbloqueo");

            migrationBuilder.DropSequence(
                name: "ordenes_reparacion_numero_seq");
        }
    }
}
