using System;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

/// <summary>Gerenciamento de usuarios do sistema. Exclusivo para ADMIN.</summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "ADMIN")]
[Produces("application/json")]
[Tags("Usuarios (Admin)")]
public class UsuariosController : ControllerBase
{
    private readonly IUsuarioService _usuarioService;
    public UsuariosController(IUsuarioService usuarioService) => _usuarioService = usuarioService;

    /// <summary>Lista todos os usuarios do sistema.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(System.Collections.Generic.IEnumerable<UsuarioResponseDto>), 200)]
    public async Task<IActionResult> GetAll()
        => Ok(await _usuarioService.ObterTodosAsync());

    /// <summary>Busca um usuario pelo ID.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(UsuarioResponseDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var u = await _usuarioService.ObterPorIdAsync(id);
        return u is null ? NotFound(new { message = "Usuario nao encontrado." }) : Ok(u);
    }

    /// <summary>Atualiza nome, perfil e status (ativo/inativo) de um usuario.</summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(UsuarioResponseDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UsuarioUpdateDto dto)
    {
        var result = await _usuarioService.AtualizarAsync(id, dto);
        return result is null ? NotFound(new { message = "Usuario nao encontrado." }) : Ok(result);
    }

    /// <summary>Remove permanentemente um usuario do sistema.</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _usuarioService.ExcluirAsync(id);
        return deleted ? NoContent() : NotFound(new { message = "Usuario nao encontrado." });
    }
}
