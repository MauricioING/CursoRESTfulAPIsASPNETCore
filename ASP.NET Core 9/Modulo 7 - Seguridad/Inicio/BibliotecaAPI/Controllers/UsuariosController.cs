using BibliotecaAPI.DTOs;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BibliotecaAPI.Controllers;

[Route("api/usuarios")]
[ApiController]
[AllowAnonymous]
public class UsuariosController : ControllerBase
{
    private readonly UserManager<IdentityUser> userManager;
    private readonly IConfiguration configuration;

    public UsuariosController(UserManager<IdentityUser> userManager,IConfiguration configuration)
    {
        this.userManager = userManager;
        this.configuration = configuration;
    }
    [HttpPost("registro")]
    public async Task<ActionResult<RespuestaAutenticacionDTO>> Registrar(CredencialesUsuarioDTO credencialesUsuarioDTO)
    {
        var usuario = new IdentityUser { UserName = credencialesUsuarioDTO.Email, Email = credencialesUsuarioDTO.Email };
        var resultado = await userManager.CreateAsync(usuario, credencialesUsuarioDTO.Password!);
        if (resultado.Succeeded)
        {
            var respuestaToken = await ConstruirToken(credencialesUsuarioDTO);
            return Ok(respuestaToken);
        }
        else
        {
            foreach (var error in resultado.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return ValidationProblem();
        }
    }
    private async Task<RespuestaAutenticacionDTO> ConstruirToken(CredencialesUsuarioDTO credencialesUsuarioDTO)
    {
        var claims = new List<Claim>()
        {
            new Claim("email", credencialesUsuarioDTO.Email),
            new Claim("lo que yo quiera", "cualquier valor")
        };

        var usuario = await userManager.FindByEmailAsync(credencialesUsuarioDTO.Email);
        var claimsDB = await userManager.GetClaimsAsync(usuario!);

        claims.AddRange(claimsDB);

        var llave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!));
        var creds = new SigningCredentials(llave, SecurityAlgorithms.HmacSha256);
        var expiracion = DateTime.UtcNow.AddYears(1);
        var securityToken = new JwtSecurityToken(issuer: null, audience: null, claims: claims, expires: expiracion, signingCredentials: creds);
        
        var token = new JwtSecurityTokenHandler().WriteToken(securityToken);

        return new RespuestaAutenticacionDTO()
        {
            Token = token,
            Expiracion = expiracion
        };
    }
}
