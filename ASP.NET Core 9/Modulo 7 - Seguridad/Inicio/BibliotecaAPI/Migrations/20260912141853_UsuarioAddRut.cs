using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BibliotecaAPI.Migrations;

/// <inheritdoc />
public partial class UsuarioAddRut : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "AsientoAsignado",
            table: "AspNetUsers",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "Cargo",
            table: "AspNetUsers",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<bool>(
            name: "EsEjecutivo",
            table: "AspNetUsers",
            type: "bit",
            nullable: false,
            defaultValue: false);

        migrationBuilder.AddColumn<bool>(
            name: "Estado",
            table: "AspNetUsers",
            type: "bit",
            nullable: false,
            defaultValue: false);

        migrationBuilder.AddColumn<string>(
            name: "Nombres",
            table: "AspNetUsers",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "PrimerApellido",
            table: "AspNetUsers",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<bool>(
            name: "PrimerLogeo",
            table: "AspNetUsers",
            type: "bit",
            nullable: false,
            defaultValue: false);

        migrationBuilder.AddColumn<string>(
            name: "Rut",
            table: "AspNetUsers",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "SegundoApellido",
            table: "AspNetUsers",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<int>(
            name: "TipoUsuario",
            table: "AspNetUsers",
            type: "int",
            nullable: false,
            defaultValue: 0);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "AsientoAsignado",
            table: "AspNetUsers");

        migrationBuilder.DropColumn(
            name: "Cargo",
            table: "AspNetUsers");

        migrationBuilder.DropColumn(
            name: "EsEjecutivo",
            table: "AspNetUsers");

        migrationBuilder.DropColumn(
            name: "Estado",
            table: "AspNetUsers");

        migrationBuilder.DropColumn(
            name: "Nombres",
            table: "AspNetUsers");

        migrationBuilder.DropColumn(
            name: "PrimerApellido",
            table: "AspNetUsers");

        migrationBuilder.DropColumn(
            name: "PrimerLogeo",
            table: "AspNetUsers");

        migrationBuilder.DropColumn(
            name: "Rut",
            table: "AspNetUsers");

        migrationBuilder.DropColumn(
            name: "SegundoApellido",
            table: "AspNetUsers");

        migrationBuilder.DropColumn(
            name: "TipoUsuario",
            table: "AspNetUsers");
    }
}
