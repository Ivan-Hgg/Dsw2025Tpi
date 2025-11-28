using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dsw2025Tpi.Data.Migrations.Authenticate
{
    /// <inheritdoc />
    public partial class RegisterChange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Usuarios_CustomerId",
                table: "Usuarios");

            migrationBuilder.AlterColumn<Guid>(
                name: "CustomerId",
                table: "Usuarios",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_CustomerId",
                table: "Usuarios",
                column: "CustomerId",
                unique: true,
                filter: "[CustomerId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Usuarios_CustomerId",
                table: "Usuarios");

            migrationBuilder.AlterColumn<Guid>(
                name: "CustomerId",
                table: "Usuarios",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_CustomerId",
                table: "Usuarios",
                column: "CustomerId",
                unique: true);
        }
    }
}
