using System.ComponentModel.DataAnnotations;

namespace BibliotecaAPI.DTOs;

public class CredencialesUsuarioDTO
{
    [Required]
    public required string Rut { get; set; }
    [Required]
    public string? Password { get; set; }
}
