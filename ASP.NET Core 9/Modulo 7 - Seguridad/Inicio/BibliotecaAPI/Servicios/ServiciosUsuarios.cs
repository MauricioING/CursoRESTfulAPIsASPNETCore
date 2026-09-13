using BibliotecaAPI.Entidades;

using Microsoft.AspNetCore.Identity;

namespace BibliotecaAPI.Servicios;

public class ServiciosUsuarios : IServiciosUsuarios
{
    private readonly UserManager<Usuario> userManager;
    private readonly IHttpContextAccessor contextAccessor;

    public ServiciosUsuarios(UserManager<Usuario> userManager, IHttpContextAccessor contextAccessor)
    {
        this.userManager = userManager;
        this.contextAccessor = contextAccessor;
    }

    public async Task<Usuario?> ObtenerUsuario()
    {
        var rutClaim = contextAccessor.HttpContext!.User.Claims.FirstOrDefault(x => x.Type == "rut");
        if (rutClaim is null)
        {
            return null;
        }
        var rut = rutClaim.Value;
        var usuario = await userManager.FindByNameAsync(rut);
        return usuario;
    }
}
