using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

/// <summary>Gerenciamento de familias pesquisadas.</summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
[Tags("Familias")]
public class FamiliasController : ControllerBase
{
    private readonly IFamiliaService _familiaService;
    public FamiliasController(IFamiliaService familiaService) => _familiaService = familiaService;

    /// <summary>Lista todas as familias. Filtravel por bairro e comunidade.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(System.Collections.Generic.IEnumerable<FamiliaResponseDto>), 200)]
    public async Task<IActionResult> GetAll([FromQuery] string? bairro, [FromQuery] string? comunidade)
        => Ok(await _familiaService.ObterTodasAsync(bairro, comunidade));

    /// <summary>Busca uma familia pelo ID.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(FamiliaResponseDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var familia = await _familiaService.ObterPorIdAsync(id);
        return familia is null ? NotFound(new { message = "Familia nao encontrada." }) : Ok(familia);
    }

    /// <summary>Cadastra uma nova familia. Requer autenticacao — o criador e registrado automaticamente.</summary>
    [HttpPost]
    [Authorize(Roles = "PESQUISADOR,ADMIN")]
    [ProducesResponseType(typeof(FamiliaResponseDto), 201)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Create([FromBody] FamiliaCreateDto dto)
    {
        var idStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(idStr, out var idUsuario))
            return Unauthorized();

        var familia = await _familiaService.CriarAsync(dto, idUsuario);
        return CreatedAtAction(nameof(GetById), new { id = familia.IdFamilia }, familia);
    }

    /// <summary>Atualiza os dados de uma familia existente.</summary>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "PESQUISADOR,GESTOR,ADMIN")]
    [ProducesResponseType(typeof(FamiliaResponseDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Update(Guid id, [FromBody] FamiliaUpdateDto dto)
    {
        var result = await _familiaService.AtualizarAsync(id, dto);
        return result is null ? NotFound(new { message = "Familia nao encontrada." }) : Ok(result);
    }

    /// <summary>Remove uma familia. Apenas ADMIN.</summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "ADMIN")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _familiaService.ExcluirAsync(id);
        return deleted ? NoContent() : NotFound(new { message = "Familia nao encontrada." });
    }
}
