using System;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

/// <summary>Dados de matricula/escolaridade dos alunos.</summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
[Tags("Matriculas")]
public class MatriculasController : ControllerBase
{
    private readonly IMatriculaService _matriculaService;
    public MatriculasController(IMatriculaService matriculaService) => _matriculaService = matriculaService;

    /// <summary>Lista as matriculas de um aluno especifico.</summary>
    [HttpGet("aluno/{idAluno:guid}")]
    [ProducesResponseType(typeof(System.Collections.Generic.IEnumerable<MatriculaResponseDto>), 200)]
    public async Task<IActionResult> GetByAluno(Guid idAluno)
        => Ok(await _matriculaService.ObterPorAlunoAsync(idAluno));

    /// <summary>Cadastra uma nova matricula para um aluno.</summary>
    [HttpPost]
    [Authorize(Roles = "PESQUISADOR,ADMIN")]
    [ProducesResponseType(typeof(MatriculaResponseDto), 201)]
    public async Task<IActionResult> Create([FromBody] MatriculaCreateDto dto)
    {
        var matricula = await _matriculaService.CriarAsync(dto);
        return StatusCode(201, matricula);
    }
}
