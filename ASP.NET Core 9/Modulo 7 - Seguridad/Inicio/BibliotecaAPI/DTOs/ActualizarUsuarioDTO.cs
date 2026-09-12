namespace BibliotecaAPI.DTOs;

public class ActualizarUsuarioDTO
{
    public string? Rut { get; set; }
    public string? Nombres { get; set; }
    public string? PrimerApellido { get; set; }
    public string? SegundoApellido { get; set; }
    public string? Cargo { get; set; }
    public string? AsientoAsignado { get; set; }
    public bool EsEjecutivo { get; set; }
    public int TipoUsuario { get; set; }
    public bool PrimerLogeo { get; set; }
    public bool Estado { get; set; } = true;
}
