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

public class MatriculaService : IMatriculaService
{
    private readonly ColetaDbContext _context;
    public MatriculaService(ColetaDbContext context) => _context = context;

    public async Task<IEnumerable<MatriculaResponseDto>> ObterPorAlunoAsync(Guid idAluno)
    {
        var matriculas = await _context.Matriculas
            .Where(m => m.IdAluno == idAluno)
            .ToListAsync();
        return matriculas.Select(MapToResponse);
    }

    public async Task<MatriculaResponseDto> CriarAsync(MatriculaCreateDto dto)
    {
        var matricula = new Matricula
        {
            IdMatricula          = Guid.NewGuid(),
            IdAluno              = dto.IdAluno,
            AnoLetivo            = dto.AnoLetivo,
            AnoSerie             = dto.AnoSerie,
            Turno                = dto.Turno,
            FrequenciaEscolarPct = dto.FrequenciaEscolarPct,
            MeioTransporteEscola = dto.MeioTransporteEscola,
            TempoDeslocamentoMin = dto.TempoDeslocamentoMin
        };
        _context.Matriculas.Add(matricula);
        await _context.SaveChangesAsync();
        return MapToResponse(matricula);
    }

    private static MatriculaResponseDto MapToResponse(Matricula m) => new()
    {
        IdMatricula          = m.IdMatricula,
        IdAluno              = m.IdAluno,
        AnoLetivo            = m.AnoLetivo,
        AnoSerie             = m.AnoSerie,
        Turno                = m.Turno,
        FrequenciaEscolarPct = m.FrequenciaEscolarPct,
        MeioTransporteEscola = m.MeioTransporteEscola,
        TempoDeslocamentoMin = m.TempoDeslocamentoMin
    };
}
