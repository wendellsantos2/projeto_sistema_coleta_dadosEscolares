using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Interfaces;
using Entities.Models;
using Infra.Context;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class AlunoService : IAlunoService
{
    private readonly ColetaDbContext _context;
    public AlunoService(ColetaDbContext context) => _context = context;

    public async Task<IEnumerable<AlunoResponseDto>> ObterTodosAsync(Guid? idFamilia = null, string? nome = null)
    {
        var query = _context.Alunos.Include(a => a.Matriculas).AsQueryable();
        if (idFamilia.HasValue) query = query.Where(a => a.IdFamilia == idFamilia);
        if (!string.IsNullOrWhiteSpace(nome)) query = query.Where(a => a.NomeAluno.ToLower().Contains(nome.ToLower()));
        return (await query.ToListAsync()).Select(MapToResponse);
    }

    public async Task<AlunoResponseDto?> ObterPorIdAsync(Guid id)
    {
        var a = await _context.Alunos.Include(a => a.Matriculas).FirstOrDefaultAsync(a => a.IdAluno == id);
        return a is null ? null : MapToResponse(a);
    }

    public async Task<AlunoResponseDto> CriarAsync(AlunoCreateDto dto)
    {
        var aluno = new Aluno
        {
            IdAluno    = dto.IdAluno ?? Guid.NewGuid(),
            IdFamilia  = dto.IdFamilia,
            NomeAluno  = dto.NomeAluno,
            DataNascimento = dto.DataNascimento,
            Sexo       = dto.Sexo,
            CpfAluno   = dto.CpfAluno,
            NecessidadeEducacionalEspecial = dto.NecessidadeEducacionalEspecial,
            DescricaoNecessidade = dto.DescricaoNecessidade,
            CreatedAt  = DateTime.UtcNow,
            UpdatedAt  = DateTime.UtcNow
        };
        _context.Alunos.Add(aluno);
        await _context.SaveChangesAsync();
        return MapToResponse(aluno);
    }

    public async Task<AlunoResponseDto?> AtualizarAsync(Guid id, AlunoUpdateDto dto)
    {
        var aluno = await _context.Alunos.Include(a => a.Matriculas).FirstOrDefaultAsync(a => a.IdAluno == id);
        if (aluno is null) return null;

        aluno.NomeAluno  = dto.NomeAluno;
        aluno.DataNascimento = dto.DataNascimento;
        aluno.Sexo       = dto.Sexo;
        aluno.CpfAluno   = dto.CpfAluno;
        aluno.NecessidadeEducacionalEspecial = dto.NecessidadeEducacionalEspecial;
        aluno.DescricaoNecessidade = dto.DescricaoNecessidade;
        aluno.UpdatedAt  = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return MapToResponse(aluno);
    }

    public async Task<bool> ExcluirAsync(Guid id)
    {
        var aluno = await _context.Alunos.FindAsync(id);
        if (aluno is null) return false;
        _context.Alunos.Remove(aluno);
        await _context.SaveChangesAsync();
        return true;
    }

    private static AlunoResponseDto MapToResponse(Aluno a) => new()
    {
        IdAluno    = a.IdAluno,
        IdFamilia  = a.IdFamilia,
        NomeAluno  = a.NomeAluno,
        DataNascimento = a.DataNascimento,
        Sexo       = a.Sexo,
        CpfAluno   = a.CpfAluno,
        NecessidadeEducacionalEspecial = a.NecessidadeEducacionalEspecial,
        DescricaoNecessidade = a.DescricaoNecessidade,
        CreatedAt  = a.CreatedAt,
        Matriculas = a.Matriculas?.Select(m => new MatriculaResponseDto
        {
            IdMatricula          = m.IdMatricula,
            IdAluno              = m.IdAluno,
            AnoLetivo            = m.AnoLetivo,
            AnoSerie             = m.AnoSerie,
            Turno                = m.Turno,
            FrequenciaEscolarPct = m.FrequenciaEscolarPct,
            MeioTransporteEscola = m.MeioTransporteEscola,
            TempoDeslocamentoMin = m.TempoDeslocamentoMin
        }).ToList() ?? new()
    };
}
