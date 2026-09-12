using System.Text.RegularExpressions;

namespace BibliotecaAPI.Utilidades;

public static class RutHelper
{
    // Normaliza el RUT: quita puntos y guiones, y pasa a mayúsculas
    public static string NormalizeRut(string? rut)
    {
        if (string.IsNullOrWhiteSpace(rut))
            return string.Empty;

        var cleaned = Regex.Replace(rut.Trim(), "[\\.\\-]", string.Empty);
        return cleaned.ToUpperInvariant();
    }
}
