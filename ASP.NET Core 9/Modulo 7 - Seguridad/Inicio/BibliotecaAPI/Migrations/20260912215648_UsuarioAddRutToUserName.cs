using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BibliotecaAPI.Migrations;

/// <inheritdoc />
public partial class UsuarioAddRutToUserName : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "SegundoApellido",
            table: "AspNetUsers",
            newName: "NombreCompleto");

        migrationBuilder.RenameColumn(
            name: "PrimerApellido",
            table: "AspNetUsers",
            newName: "Apelldos");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "NombreCompleto",
            table: "AspNetUsers",
            newName: "SegundoApellido");

        migrationBuilder.RenameColumn(
            name: "Apelldos",
            table: "AspNetUsers",
            newName: "PrimerApellido");
    }
}
