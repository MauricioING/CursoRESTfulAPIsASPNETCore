using BibliotecaAPI.DTOs;
using BibliotecaAPI.Entidades;

using Microsoft.AspNetCore.Authorization;
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
    private readonly UserManager<Usuario> userManager;
    private readonly IConfiguration configuration;
    private readonly SignInManager<Usuario> signInManager;
    private readonly Task<Usuario> serviciosUsuarios;

    public UsuariosController(UserManager<Usuario> userManager, IConfiguration configuration, SignInManager<Usuario> signInManager)
    {
        this.userManager = userManager;
        this.configuration = configuration;
        this.signInManager = signInManager;
    }
    [HttpPost("registro")]
    public async Task<ActionResult<RespuestaAutenticacionDTO>> Registrar(CredencialesUsuarioDTO credencialesUsuarioDTO)
    {
        var usuario = new Usuario { UserName = credencialesUsuarioDTO.Email, Email = credencialesUsuarioDTO.Email };
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
    [HttpPost("login")]
    public async Task<ActionResult<RespuestaAutenticacionDTO>> Login(CredencialesUsuarioDTO credencialesUsuarioDTO)
    {
        var usuario = await userManager.FindByEmailAsync(credencialesUsuarioDTO.Email);
        if (usuario is null)
        {
            return RetornarLoginIncorrecto();
        }

        var resultado = await signInManager.CheckPasswordSignInAsync(usuario, credencialesUsuarioDTO.Password!, false);
        if (resultado.Succeeded)
        {
            return await ConstruirToken(credencialesUsuarioDTO);
        }
        else
        {
            return RetornarLoginIncorrecto();
        }
    }
    [HttpPut]
    public async Task<ActionResult> Put(ActualizarUsuarioDTO actualizarUsuarioDTO)
    {
        var usuario = await Task.FromResult(new Usuario());
        if (usuario is null)
        {
            return NotFound();
        }

        usuario.Rut = actualizarUsuarioDTO.Rut;
        usuario.Nombres = actualizarUsuarioDTO.Nombres;
        usuario.PrimerApellido = actualizarUsuarioDTO.PrimerApellido;
        usuario.SegundoApellido = actualizarUsuarioDTO.SegundoApellido;
        usuario.Cargo = actualizarUsuarioDTO.Cargo;
        usuario.AsientoAsignado = actualizarUsuarioDTO.AsientoAsignado;
        usuario.EsEjecutivo = actualizarUsuarioDTO.EsEjecutivo;
        usuario.TipoUsuario = actualizarUsuarioDTO.TipoUsuario;
        usuario.PrimerLogeo = actualizarUsuarioDTO.PrimerLogeo;
        usuario.Estado = actualizarUsuarioDTO.Estado;

        var resultado = await userManager.UpdateAsync(usuario);
        if (resultado.Succeeded)
        {
            return NoContent();
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
    private ActionResult RetornarLoginIncorrecto()
    {
        ModelState.AddModelError(string.Empty, "Login incorrecto");
        return ValidationProblem();
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
