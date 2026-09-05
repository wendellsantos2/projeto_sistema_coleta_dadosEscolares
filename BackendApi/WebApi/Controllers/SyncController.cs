using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SyncController : ControllerBase
{
    private readonly ISyncService _syncService;

    public SyncController(ISyncService syncService)
    {
        _syncService = syncService;
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] List<ColetaSyncDto> coletas)
    {
        if (coletas == null || coletas.Count == 0)
            return BadRequest("O array de sincronização está vazio.");

        try
        {
            await _syncService.SincronizarLoteAsync(coletas);
            return Ok(new { message = $"Sincronização de {coletas.Count} registros realizada com sucesso!" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro ao sincronizar dados.", error = ex.Message });
        }
    }
}
