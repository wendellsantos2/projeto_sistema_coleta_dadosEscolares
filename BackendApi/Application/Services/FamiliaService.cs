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

public class FamiliaService : IFamiliaService
{
    private readonly ColetaDbContext _context;
    public FamiliaService(ColetaDbContext context) => _context = context;

    public async Task<IEnumerable<FamiliaResponseDto>> ObterTodasAsync(string? bairro = null, string? comunidade = null)
    {
        var query = _context.Familias
            .Include(f => f.UsuarioCriador)
            .Include(f => f.Alunos)
                .ThenInclude(a => a.Matriculas)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(bairro))
            query = query.Where(f => f.Bairro.ToLower().Contains(bairro.ToLower()));
        if (!string.IsNullOrWhiteSpace(comunidade))
            query = query.Where(f => f.Comunidade.ToLower().Contains(comunidade.ToLower()));

        return (await query.ToListAsync()).Select(MapToResponse);
    }

    public async Task<FamiliaResponseDto?> ObterPorIdAsync(Guid id)
    {
        var f = await _context.Familias
            .Include(f => f.UsuarioCriador)
            .Include(f => f.Alunos).ThenInclude(a => a.Matriculas)
            .FirstOrDefaultAsync(f => f.IdFamilia == id);
        return f is null ? null : MapToResponse(f);
    }

    public async Task<FamiliaResponseDto> CriarAsync(FamiliaCreateDto dto, Guid idUsuario)
    {
        var familia = new Familia
        {
            IdFamilia          = Guid.NewGuid(),
            CodigoFamilia      = dto.CodigoFamilia,
            Endereco           = dto.Endereco,
            Bairro             = dto.Bairro,
            Comunidade         = dto.Comunidade,
            QtdMoradores       = dto.QtdMoradores,
            RendaFamiliarMensal = dto.RendaFamiliarMensal,
            RecebeBeneficioSocial = dto.RecebeBeneficioSocial,
            BeneficioSocial    = dto.BeneficioSocial,
            PossuiInternetCasa = dto.PossuiInternetCasa,
            TipoAcessoInternet = dto.TipoAcessoInternet,
            CriadoPor          = idUsuario,
            CreatedAt          = DateTime.UtcNow,
            UpdatedAt          = DateTime.UtcNow
        };
        _context.Familias.Add(familia);
        await _context.SaveChangesAsync();

        await _context.Entry(familia).Reference(f => f.UsuarioCriador).LoadAsync();
        return MapToResponse(familia);
    }

    public async Task<FamiliaResponseDto?> AtualizarAsync(Guid id, FamiliaUpdateDto dto)
    {
        var familia = await _context.Familias.Include(f => f.UsuarioCriador)
            .Include(f => f.Alunos).ThenInclude(a => a.Matriculas)
            .FirstOrDefaultAsync(f => f.IdFamilia == id);
        if (familia is null) return null;

        familia.Endereco            = dto.Endereco;
        familia.Bairro              = dto.Bairro;
        familia.Comunidade          = dto.Comunidade;
        familia.QtdMoradores        = dto.QtdMoradores;
        familia.RendaFamiliarMensal = dto.RendaFamiliarMensal;
        familia.RecebeBeneficioSocial = dto.RecebeBeneficioSocial;
        familia.BeneficioSocial     = dto.BeneficioSocial;
        familia.PossuiInternetCasa  = dto.PossuiInternetCasa;
        familia.TipoAcessoInternet  = dto.TipoAcessoInternet;
        familia.UpdatedAt           = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return MapToResponse(familia);
    }

    public async Task<bool> ExcluirAsync(Guid id)
    {
        var familia = await _context.Familias.FindAsync(id);
        if (familia is null) return false;
        _context.Familias.Remove(familia);
        await _context.SaveChangesAsync();
        return true;
    }

    private static FamiliaResponseDto MapToResponse(Familia f) => new()
    {
        IdFamilia             = f.IdFamilia,
        CodigoFamilia         = f.CodigoFamilia,
        Endereco              = f.Endereco,
        Bairro                = f.Bairro,
        Comunidade            = f.Comunidade,
        QtdMoradores          = f.QtdMoradores,
        RendaFamiliarMensal   = f.RendaFamiliarMensal,
        RecebeBeneficioSocial = f.RecebeBeneficioSocial,
        BeneficioSocial       = f.BeneficioSocial,
        PossuiInternetCasa    = f.PossuiInternetCasa,
        TipoAcessoInternet    = f.TipoAcessoInternet,
        CriadoPorNome         = f.UsuarioCriador?.Nome ?? "",
        CreatedAt             = f.CreatedAt,
        Alunos = f.Alunos?.Select(a => new AlunoResponseDto
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
                IdMatricula           = m.IdMatricula,
                IdAluno               = m.IdAluno,
                AnoLetivo             = m.AnoLetivo,
                AnoSerie              = m.AnoSerie,
                Turno                 = m.Turno,
                FrequenciaEscolarPct  = m.FrequenciaEscolarPct,
                MeioTransporteEscola  = m.MeioTransporteEscola,
                TempoDeslocamentoMin  = m.TempoDeslocamentoMin
            }).ToList() ?? new()
        }).ToList() ?? new()
    };
}
