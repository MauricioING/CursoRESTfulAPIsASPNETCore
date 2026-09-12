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
        var emailClaim = contextAccessor.HttpContext!.User.Claims.FirstOrDefault(x => x.Type == "email");
        if (emailClaim is null)
        {
            return null;
        }
        var email = emailClaim.Value;
        var usuario = await userManager.FindByEmailAsync(email);
        return usuario;
    }
}
