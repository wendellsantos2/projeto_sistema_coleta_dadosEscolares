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
                NomeAluno           = r.Aluno != null ? r.Aluno.NomeAluno : "",
                CpfAluno            = r.Aluno != null ? r.Aluno.CpfAluno : "",
                DataNascimento      = r.Aluno != null ? r.Aluno.DataNascimento : null,
                NecessidadeEducacionalEspecial = r.Aluno != null && r.Aluno.NecessidadeEducacionalEspecial,
                
                IdFamilia           = r.IdFamilia,
                CodigoFamilia       = r.Familia != null ? r.Familia.CodigoFamilia : "",
                Endereco            = r.Familia != null ? r.Familia.Endereco : "",
                Bairro              = r.Familia != null ? r.Familia.Bairro : "",
                Comunidade          = r.Familia != null ? r.Familia.Comunidade : "",
                PossuiInternetCasa  = r.Familia != null && r.Familia.PossuiInternetCasa,
                TipoAcessoInternet  = r.Familia != null ? r.Familia.TipoAcessoInternet : null,
                RecebeBeneficioSocial = r.Familia != null && r.Familia.RecebeBeneficioSocial,
                BeneficioSocial     = r.Familia != null ? r.Familia.BeneficioSocial : null,
                RendaFamiliarMensal = r.Familia != null ? r.Familia.RendaFamiliarMensal : 0,

                PesquisadorNome     = r.Usuario != null ? r.Usuario.Nome : "",
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
            NomeAluno           = r.Aluno != null ? r.Aluno.NomeAluno : "",
            CpfAluno            = r.Aluno != null ? r.Aluno.CpfAluno : "",
            DataNascimento      = r.Aluno != null ? r.Aluno.DataNascimento : null,
            NecessidadeEducacionalEspecial = r.Aluno != null && r.Aluno.NecessidadeEducacionalEspecial,
            
            IdFamilia           = r.IdFamilia,
            CodigoFamilia       = r.Familia != null ? r.Familia.CodigoFamilia : "",
            Endereco            = r.Familia != null ? r.Familia.Endereco : "",
            Bairro              = r.Familia != null ? r.Familia.Bairro : "",
            Comunidade          = r.Familia != null ? r.Familia.Comunidade : "",
            PossuiInternetCasa  = r.Familia != null && r.Familia.PossuiInternetCasa,
            TipoAcessoInternet  = r.Familia != null ? r.Familia.TipoAcessoInternet : null,
            RecebeBeneficioSocial = r.Familia != null && r.Familia.RecebeBeneficioSocial,
            BeneficioSocial     = r.Familia != null ? r.Familia.BeneficioSocial : null,
            RendaFamiliarMensal = r.Familia != null ? r.Familia.RendaFamiliarMensal : 0,

            PesquisadorNome     = r.Usuario != null ? r.Usuario.Nome : "",
            Observacao          = r.Observacao,
            StatusSincronizacao = r.StatusSincronizacao,
            DataColeta          = r.DataColeta,
            SincronizadoEm     = r.SincronizadoEm
        };
    }

    public async Task<DashboardDto> ObterDashboardAsync(string? bairro = null, string? turno = null)
    {
        var familiasQuery = _context.Familias.AsQueryable();
        var alunosQuery = _context.Alunos.AsQueryable();
        var matriculasQuery = _context.Matriculas.AsQueryable();
        var registrosQuery = _context.RegistrosColeta.AsQueryable();

        // Aplicar filtros
        if (!string.IsNullOrWhiteSpace(bairro))
        {
            familiasQuery = familiasQuery.Where(f => f.Bairro == bairro);
            var familiasIds = await familiasQuery.Select(f => f.IdFamilia).ToListAsync();
            alunosQuery = alunosQuery.Where(a => familiasIds.Contains(a.IdFamilia));
            registrosQuery = registrosQuery.Where(r => r.IdFamilia.HasValue && familiasIds.Contains(r.IdFamilia.Value));
            var alunosIds = await alunosQuery.Select(a => a.IdAluno).ToListAsync();
            matriculasQuery = matriculasQuery.Where(m => alunosIds.Contains(m.IdAluno));
        }

        if (!string.IsNullOrWhiteSpace(turno))
        {
            matriculasQuery = matriculasQuery.Where(m => m.Turno == turno);
            var alunosIds = await matriculasQuery.Select(m => m.IdAluno).Distinct().ToListAsync();
            alunosQuery = alunosQuery.Where(a => alunosIds.Contains(a.IdAluno));
            var familiasIds = await alunosQuery.Select(a => a.IdFamilia).Distinct().ToListAsync();
            familiasQuery = familiasQuery.Where(f => familiasIds.Contains(f.IdFamilia));
            registrosQuery = registrosQuery.Where(r => r.IdFamilia.HasValue && familiasIds.Contains(r.IdFamilia.Value));
        }

        var totalAlunos   = await alunosQuery.CountAsync();
        var totalFamilias = await familiasQuery.CountAsync();
        var totalPesq     = await registrosQuery.Select(r => r.IdAluno).Distinct().CountAsync();
        var comNee        = await alunosQuery.CountAsync(a => a.NecessidadeEducacionalEspecial);
        var comBeneficio  = await familiasQuery.CountAsync(f => f.RecebeBeneficioSocial);
        var semInternet   = await familiasQuery.CountAsync(f => !f.PossuiInternetCasa);
        var freqMedia     = await matriculasQuery.AnyAsync()
            ? await matriculasQuery.AverageAsync(m => m.FrequenciaEscolarPct)
            : 0;

        var transporte = await matriculasQuery
            .GroupBy(m => m.MeioTransporteEscola)
            .Select(g => new TransporteDistribuicaoDto { Tipo = g.Key, Quantidade = g.Count() })
            .ToArrayAsync();

        var turnoDist = await matriculasQuery
            .GroupBy(m => m.Turno)
            .Select(g => new TurnoDistribuicaoDto { Turno = g.Key, Quantidade = g.Count() })
            .ToArrayAsync();

        var beneficios = await familiasQuery
            .Where(f => f.RecebeBeneficioSocial && f.BeneficioSocial != null)
            .GroupBy(f => f.BeneficioSocial!)
            .Select(g => new BeneficioDistribuicaoDto { Beneficio = g.Key, Quantidade = g.Count() })
            .ToArrayAsync();

        var renda = (await familiasQuery.Select(f => f.RendaFamiliarMensal).ToListAsync())
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
            DistribuicaoTurno           = turnoDist,
            DistribuicaoBeneficios      = beneficios,
            DistribuicaoRenda           = renda
        };
    }
}
