using System.ComponentModel.DataAnnotations;

namespace BibliotecaAPI.DTOs;

public class EditarClaimDTO
{
    [Required]
    public required string Rut { get; set; } 
}
