using System;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

/// <summary>Registros de coleta e indicadores do dashboard.</summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
[Tags("Registros e Dashboard")]
public class RegistrosColetaController : ControllerBase
{
    private readonly IRegistroColetaService _registroService;
    public RegistrosColetaController(IRegistroColetaService registroService) => _registroService = registroService;

    /// <summary>Lista todos os registros de coleta. Filtravel por status: PENDENTE ou SINCRONIZADO.</summary>
    [HttpGet]
    [Authorize(Roles = "GESTOR,ADMIN")]
    [ProducesResponseType(typeof(System.Collections.Generic.IEnumerable<RegistroColetaResponseDto>), 200)]
    public async Task<IActionResult> GetAll([FromQuery] string? status)
        => Ok(await _registroService.ObterTodosAsync(status));

    /// <summary>Busca um registro de coleta pelo ID.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(RegistroColetaResponseDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var r = await _registroService.ObterPorIdAsync(id);
        return r is null ? NotFound(new { message = "Registro nao encontrado." }) : Ok(r);
    }

    /// <summary>Retorna os indicadores agregados para o dashboard (totais, graficos, distribuicoes).</summary>
    [HttpGet("dashboard")]
    [Authorize(Roles = "GESTOR,ADMIN")]
    [ProducesResponseType(typeof(DashboardDto), 200)]
    public async Task<IActionResult> Dashboard([FromQuery] string? bairro, [FromQuery] string? turno)
        => Ok(await _registroService.ObterDashboardAsync(bairro, turno));
}
