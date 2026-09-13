using Microsoft.AspNetCore.Identity;

namespace BibliotecaAPI.Entidades;

public class Usuario : IdentityUser
{
    // RUT del usuario (ej.: 12.345.678-9). Se almacenará además en UserName normalizado.
    // Identificador tributario chileno del usuario.
    // Campo opcional para identificar al usuario por su documento nacional.
    public required string Rut { get; set; }
    public string? Nombres { get; set; }
    public string? Apellidos { get; set; }
    public string? NombreCompleto { get; set; }
    public string? Cargo { get; set; }
    public string? AsientoAsignado { get; set; }
    public bool EsEjecutivo { get; set; }
    public int TipoUsuario { get; set; }
    public bool PrimerLogeo { get; set; }
    public bool Estado { get; set; } = true;
}
