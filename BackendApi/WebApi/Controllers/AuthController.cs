using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

/// <summary>Autenticacao e gerenciamento de sessao JWT.</summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Tags("Auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IUsuarioService _usuarioService;

    public AuthController(IAuthService authService, IUsuarioService usuarioService)
    {
        _authService    = authService;
        _usuarioService = usuarioService;
    }

    /// <summary>Autentica o usuario e retorna o JWT.</summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(TokenDto), 200)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        try
        {
            var token = await _authService.LoginAsync(dto);
            return Ok(token);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }

    /// <summary>Cadastra um novo usuario no sistema.</summary>
    [HttpPost("cadastrar")]
    [ProducesResponseType(201)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Cadastrar([FromBody] CadastroUsuarioDto dto)
    {
        try
        {
            await _authService.CadastrarAsync(dto);
            return StatusCode(201, new { message = "Usuario cadastrado com sucesso!" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>Retorna os dados do usuario autenticado (perfil proprio).</summary>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(UsuarioResponseDto), 200)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> Me()
    {
        var idStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(idStr, out var id))
            return Unauthorized();

        var perfil = await _usuarioService.ObterPerfilAsync(id);
        return perfil is null ? NotFound() : Ok(perfil);
    }
}
