using System;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

/// <summary>Gerenciamento de alunos cadastrados.</summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
[Tags("Alunos")]
public class AlunosController : ControllerBase
{
    private readonly IAlunoService _alunoService;
    public AlunosController(IAlunoService alunoService) => _alunoService = alunoService;

    /// <summary>Lista todos os alunos. Filtravel por idFamilia e nome.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(System.Collections.Generic.IEnumerable<AlunoResponseDto>), 200)]
    public async Task<IActionResult> GetAll([FromQuery] Guid? idFamilia, [FromQuery] string? nome)
        => Ok(await _alunoService.ObterTodosAsync(idFamilia, nome));

    /// <summary>Busca um aluno pelo ID.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(AlunoResponseDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var aluno = await _alunoService.ObterPorIdAsync(id);
        return aluno is null ? NotFound(new { message = "Aluno nao encontrado." }) : Ok(aluno);
    }

    /// <summary>Cadastra um novo aluno vinculado a uma familia.</summary>
    [HttpPost]
    [Authorize(Roles = "PESQUISADOR,ADMIN")]
    [ProducesResponseType(typeof(AlunoResponseDto), 201)]
    public async Task<IActionResult> Create([FromBody] AlunoCreateDto dto)
    {
        var aluno = await _alunoService.CriarAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = aluno.IdAluno }, aluno);
    }

    /// <summary>Atualiza dados de um aluno existente.</summary>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "PESQUISADOR,GESTOR,ADMIN")]
    [ProducesResponseType(typeof(AlunoResponseDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Update(Guid id, [FromBody] AlunoUpdateDto dto)
    {
        var result = await _alunoService.AtualizarAsync(id, dto);
        return result is null ? NotFound(new { message = "Aluno nao encontrado." }) : Ok(result);
    }

    /// <summary>Remove um aluno. Apenas ADMIN.</summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "ADMIN")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _alunoService.ExcluirAsync(id);
        return deleted ? NoContent() : NotFound(new { message = "Aluno nao encontrado." });
    }
}
