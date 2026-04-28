using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Workshop.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddOrdenDeTrabajo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MantenimientoItems_Productos_ProductoId",
                table: "MantenimientoItems");

            migrationBuilder.AddColumn<string>(
                name: "Descripcion",
                table: "Mantenimientos",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Diagnostico",
                table: "Mantenimientos",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaEntrega",
                table: "Mantenimientos",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Folio",
                table: "Mantenimientos",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Observaciones",
                table: "Mantenimientos",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<Guid>(
                name: "ProductoId",
                table: "MantenimientoItems",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<string>(
                name: "Nombre",
                table: "MantenimientoItems",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Tipo",
                table: "MantenimientoItems",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_MantenimientoItems_Productos_ProductoId",
                table: "MantenimientoItems",
                column: "ProductoId",
                principalTable: "Productos",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MantenimientoItems_Productos_ProductoId",
                table: "MantenimientoItems");

            migrationBuilder.DropColumn(
                name: "Descripcion",
                table: "Mantenimientos");

            migrationBuilder.DropColumn(
                name: "Diagnostico",
                table: "Mantenimientos");

            migrationBuilder.DropColumn(
                name: "FechaEntrega",
                table: "Mantenimientos");

            migrationBuilder.DropColumn(
                name: "Folio",
                table: "Mantenimientos");

            migrationBuilder.DropColumn(
                name: "Observaciones",
                table: "Mantenimientos");

            migrationBuilder.DropColumn(
                name: "Nombre",
                table: "MantenimientoItems");

            migrationBuilder.DropColumn(
                name: "Tipo",
                table: "MantenimientoItems");

            migrationBuilder.AlterColumn<Guid>(
                name: "ProductoId",
                table: "MantenimientoItems",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_MantenimientoItems_Productos_ProductoId",
                table: "MantenimientoItems",
                column: "ProductoId",
                principalTable: "Productos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
