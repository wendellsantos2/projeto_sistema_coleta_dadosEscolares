using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Interfaces;
using Infra.Context;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class RegistroColetaService : IRegistroColetaService
{
    private readonly ColetaDbContext _context;
    public RegistroColetaService(ColetaDbContext context) => _context = context;

    public async Task<IEnumerable<RegistroColetaResponseDto>> ObterTodosAsync(string? status = null)
    {
        var query = _context.RegistrosColeta
            .Include(r => r.Aluno)
            .Include(r => r.Familia)
            .Include(r => r.Usuario)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(r => r.StatusSincronizacao == status.ToUpper());

        return (await query.OrderByDescending(r => r.DataColeta).ToListAsync())
            .Select(r => new RegistroColetaResponseDto
            {
                IdRegistro          = r.IdRegistro,
                IdAluno             = r.IdAluno,
                NomeAluno           = r.Aluno?.NomeAluno ?? "",
                IdFamilia           = r.IdFamilia,
                CodigoFamilia       = r.Familia?.CodigoFamilia ?? "",
                PesquisadorNome     = r.Usuario?.Nome ?? "",
                Observacao          = r.Observacao,
                StatusSincronizacao = r.StatusSincronizacao,
                DataColeta          = r.DataColeta,
                SincronizadoEm     = r.SincronizadoEm
            });
    }

    public async Task<RegistroColetaResponseDto?> ObterPorIdAsync(Guid id)
    {
        var r = await _context.RegistrosColeta
            .Include(r => r.Aluno)
            .Include(r => r.Familia)
            .Include(r => r.Usuario)
            .FirstOrDefaultAsync(r => r.IdRegistro == id);
        if (r is null) return null;

        return new RegistroColetaResponseDto
        {
            IdRegistro          = r.IdRegistro,
            IdAluno             = r.IdAluno,
            NomeAluno           = r.Aluno?.NomeAluno ?? "",
            IdFamilia           = r.IdFamilia,
            CodigoFamilia       = r.Familia?.CodigoFamilia ?? "",
            PesquisadorNome     = r.Usuario?.Nome ?? "",
            Observacao          = r.Observacao,
            StatusSincronizacao = r.StatusSincronizacao,
            DataColeta          = r.DataColeta,
            SincronizadoEm     = r.SincronizadoEm
        };
    }

    public async Task<DashboardDto> ObterDashboardAsync()
    {
        var totalAlunos   = await _context.Alunos.CountAsync();
        var totalFamilias = await _context.Familias.CountAsync();
        var totalPesq     = await _context.RegistrosColeta.Select(r => r.IdAluno).Distinct().CountAsync();
        var comNee        = await _context.Alunos.CountAsync(a => a.NecessidadeEducacionalEspecial);
        var comBeneficio  = await _context.Familias.CountAsync(f => f.RecebeBeneficioSocial);
        var semInternet   = await _context.Familias.CountAsync(f => !f.PossuiInternetCasa);
        var freqMedia     = await _context.Matriculas.AnyAsync()
            ? await _context.Matriculas.AverageAsync(m => m.FrequenciaEscolarPct)
            : 0;

        var transporte = await _context.Matriculas
            .GroupBy(m => m.MeioTransporteEscola)
            .Select(g => new TransporteDistribuicaoDto { Tipo = g.Key, Quantidade = g.Count() })
            .ToArrayAsync();

        var turno = await _context.Matriculas
            .GroupBy(m => m.Turno)
            .Select(g => new TurnoDistribuicaoDto { Turno = g.Key, Quantidade = g.Count() })
            .ToArrayAsync();

        var beneficios = await _context.Familias
            .Where(f => f.RecebeBeneficioSocial && f.BeneficioSocial != null)
            .GroupBy(f => f.BeneficioSocial!)
            .Select(g => new BeneficioDistribuicaoDto { Beneficio = g.Key, Quantidade = g.Count() })
            .ToArrayAsync();

        var renda = (await _context.Familias.Select(f => f.RendaFamiliarMensal).ToListAsync())
            .GroupBy(r => r <= 1500 ? "Ate R$1.500" : r <= 3000 ? "R$1.501 a R$3.000" : "Acima de R$3.000")
            .Select(g => new RendaDistribuicaoDto { Faixa = g.Key, Quantidade = g.Count() })
            .ToArray();

        return new DashboardDto
        {
            TotalAlunos                 = totalAlunos,
            TotalFamilias               = totalFamilias,
            TotalPesquisados            = totalPesq,
            AlunosComNecessidadeEspecial = comNee,
            FrequenciaMediaGeral        = Math.Round(freqMedia, 2),
            FamiliasComBeneficio        = comBeneficio,
            FamiliasSemInternet         = semInternet,
            DistribuicaoTransporte      = transporte,
            DistribuicaoTurno           = turno,
            DistribuicaoBeneficios      = beneficios,
            DistribuicaoRenda           = renda
        };
    }
}
