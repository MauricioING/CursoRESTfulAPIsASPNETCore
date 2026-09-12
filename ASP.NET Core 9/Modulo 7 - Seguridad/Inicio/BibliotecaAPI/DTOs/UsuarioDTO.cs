namespace BibliotecaAPI.DTOs;

public class UsuarioDTO
{
    public required string Email { get; set; }
    public string? Rut { get; set; }
    public string? Nombres { get; set; }
}
