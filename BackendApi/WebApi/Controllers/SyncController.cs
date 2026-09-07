using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

/// <summary>Sincronizacao em lote dos registros coletados offline no mobile.</summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "PESQUISADOR,ADMIN")]
[Produces("application/json")]
[Tags("Sincronizacao (Mobile)")]
public class SyncController : ControllerBase
{
    private readonly ISyncService _syncService;
    public SyncController(ISyncService syncService) => _syncService = syncService;

    /// <summary>
    /// Recebe um array de registros coletados offline e sincroniza com o banco central.
    /// Registros ja sincronizados (mesmo IdRegistro) sao ignorados automaticamente (idempotente).
    /// </summary>
    [HttpPost]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> Sincronizar([FromBody] List<ColetaSyncDto> coletas)
    {
        if (coletas is null || coletas.Count == 0)
            return BadRequest(new { message = "Nenhum registro para sincronizar." });

        var idStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        Guid.TryParse(idStr, out var idUsuario);

        try
        {
            await _syncService.SincronizarLoteAsync(coletas, idUsuario);
            return Ok(new { message = $"{coletas.Count} registro(s) sincronizado(s) com sucesso." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro ao sincronizar.", error = ex.Message });
        }
    }
}
