using AutoMapper;

using BibliotecaAPI.Datos;
using BibliotecaAPI.DTOs;
using BibliotecaAPI.Entidades;
using BibliotecaAPI.Servicios;
using BibliotecaAPI.Utilidades;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BibliotecaAPI.Controllers;

[Route("api/usuarios")]
[ApiController]
public class UsuariosController : ControllerBase
{
    private readonly UserManager<Usuario> userManager;
    private readonly IConfiguration configuration;
    private readonly SignInManager<Usuario> signInManager;
    private readonly IServiciosUsuarios serviciosUsuarios;
    private readonly ApplicationDbContext context;
    private readonly IMapper mapper;

    public UsuariosController(UserManager<Usuario> userManager, IConfiguration configuration, SignInManager<Usuario> signInManager,
        IServiciosUsuarios serviciosUsuarios, ApplicationDbContext context, IMapper mapper)
    {
        this.userManager = userManager;
        this.configuration = configuration;
        this.signInManager = signInManager;
        this.serviciosUsuarios = serviciosUsuarios;
        this.context = context;
        this.mapper = mapper;
    }
    [HttpGet("renovar-token")]
    [Authorize]
    public async Task<ActionResult<RespuestaAutenticacionDTO>> RenovarToken()
    {
        var usuario = await serviciosUsuarios.ObtenerUsuario();
        if (usuario is null)
        {
            return NotFound();
        }
        var credencialesUsuarioDTO = new CredencialesUsuarioDTO()
        {
            Rut = usuario.Rut!
        };
        return await ConstruirToken(credencialesUsuarioDTO);
    }
    [HttpGet]
    [Authorize(Policy = "esadmin")]
    public async Task<IEnumerable<UsuarioDTO>> Get()
    {
        var usuarios = await context.Users.ToListAsync();
        var usuarioDTO = mapper.Map<IEnumerable<UsuarioDTO>>(usuarios);
        return usuarioDTO;  
    }
    [HttpPost("registro")]
    public async Task<ActionResult<RespuestaAutenticacionDTO>> Registrar(CredencialesUsuarioDTO credencialesUsuarioDTO)
    {
        var usuario = new Usuario { UserName = RutHelper.NormalizeRut(credencialesUsuarioDTO.Rut)};
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
        var usuario = await userManager.FindByNameAsync(RutHelper.NormalizeRut(credencialesUsuarioDTO.Rut));
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
        var usuario = await userManager.FindByNameAsync(RutHelper.NormalizeRut(actualizarUsuarioDTO.Rut!));
        if (usuario is null)
        {
            return NotFound();
        }

        usuario.Rut = RutHelper.NormalizeRut(actualizarUsuarioDTO.Rut);
        usuario.Nombres = actualizarUsuarioDTO.Nombres;
        usuario.Apelldos = actualizarUsuarioDTO.Apellidos;
        usuario.NombreCompleto = $"{actualizarUsuarioDTO.Nombres} {actualizarUsuarioDTO.Apellidos}";
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
    [HttpPost("hacer-admin")]
    //[Authorize(Policy = "esadmin")]
    public async Task<ActionResult> HacerAdmin(EditarClaimDTO editarClaimDTO)
    {
        var usuario = await userManager.FindByNameAsync(RutHelper.NormalizeRut(editarClaimDTO.Rut));
        if (usuario is null)
        {
            return NotFound();
        }

        var resultado = await userManager.AddClaimAsync(usuario, new Claim("esadmin", "true"));
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
    [HttpPost("remove-esadmin")]
    [Authorize(Policy = "esadmin")]
    public async Task<ActionResult> RemoverAdmin(EditarClaimDTO editarClaimDTO)
    {
        var usuario = await userManager.FindByNameAsync(RutHelper.NormalizeRut(editarClaimDTO.Rut));
        if (usuario is null)
        {
            return NotFound();
        }

        var resultado = await userManager.RemoveClaimAsync(usuario, new Claim("esadmin", "true"));
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
            new Claim("rut", RutHelper.NormalizeRut(credencialesUsuarioDTO.Rut)),
            new Claim("lo que yo quiera", "cualquier valor")
        };

        var usuario = await userManager.FindByNameAsync(RutHelper.NormalizeRut(credencialesUsuarioDTO.Rut));
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
