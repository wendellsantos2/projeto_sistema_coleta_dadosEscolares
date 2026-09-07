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
                .ThenInclude(a => a.Matriculas)
            .Include(r => r.Familia)
                .ThenInclude(f => f.Responsaveis)
            .Include(r => r.Usuario)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(r => r.StatusSincronizacao == status.ToUpper());

        return (await query.OrderByDescending(r => r.DataColeta).ToListAsync())
            .Select(r => new RegistroColetaResponseDto
            {
                IdRegistro          = r.IdRegistro,
                IdFamilia           = r.IdFamilia,
                IdAluno             = r.IdAluno,
                NomeAluno           = r.Aluno != null ? r.Aluno.NomeAluno : "",
                DataNascimento      = r.Aluno != null ? r.Aluno.DataNascimento : null,
                Sexo                = r.Aluno != null ? r.Aluno.Sexo : "",
                CpfAluno            = r.Aluno != null ? r.Aluno.CpfAluno : "",
                
                NomeResponsavel       = r.Familia != null && r.Familia.Responsaveis.Any() ? r.Familia.Responsaveis.First().NomeResponsavel : "",
                ParentescoResponsavel = r.Familia != null && r.Familia.Responsaveis.Any() ? r.Familia.Responsaveis.First().ParentescoResponsavel : "",
                CpfResponsavel        = r.Familia != null && r.Familia.Responsaveis.Any() ? r.Familia.Responsaveis.First().CpfResponsavel : "",
                TelefoneResponsavel   = r.Familia != null && r.Familia.Responsaveis.Any() ? r.Familia.Responsaveis.First().TelefoneResponsavel : "",
                EmailResponsavel      = r.Familia != null && r.Familia.Responsaveis.Any() ? r.Familia.Responsaveis.First().EmailResponsavel : "",

                Endereco            = r.Familia != null ? r.Familia.Endereco : "",
                Bairro              = r.Familia != null ? r.Familia.Bairro : "",
                Comunidade          = r.Familia != null ? r.Familia.Comunidade : "",
                QtdMoradores        = r.Familia != null ? r.Familia.QtdMoradores : 0,
                RendaFamiliarMensal = r.Familia != null ? r.Familia.RendaFamiliarMensal : 0,
                RecebeBeneficioSocial = r.Familia != null && r.Familia.RecebeBeneficioSocial,
                BeneficioSocial     = r.Familia != null ? r.Familia.BeneficioSocial : null,
                PossuiInternetCasa  = r.Familia != null && r.Familia.PossuiInternetCasa,
                TipoAcessoInternet  = r.Familia != null ? r.Familia.TipoAcessoInternet : null,

                MeioTransporteEscola = r.Aluno != null && r.Aluno.Matriculas.Any() ? r.Aluno.Matriculas.First().MeioTransporteEscola : "",
                TempoDeslocamentoMin = r.Aluno != null && r.Aluno.Matriculas.Any() ? r.Aluno.Matriculas.First().TempoDeslocamentoMin : 0,
                FrequenciaEscolarPct = r.Aluno != null && r.Aluno.Matriculas.Any() ? r.Aluno.Matriculas.First().FrequenciaEscolarPct : 0,
                AnoSerie             = r.Aluno != null && r.Aluno.Matriculas.Any() ? r.Aluno.Matriculas.First().AnoSerie : "",
                Turno                = r.Aluno != null && r.Aluno.Matriculas.Any() ? r.Aluno.Matriculas.First().Turno : "",

                NecessidadeEducacionalEspecial = r.Aluno != null && r.Aluno.NecessidadeEducacionalEspecial,
                DescricaoNecessidade = r.Aluno != null ? r.Aluno.DescricaoNecessidade : null,

                CodigoFamilia       = r.Familia != null ? r.Familia.CodigoFamilia : "",
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
                .ThenInclude(a => a.Matriculas)
            .Include(r => r.Familia)
                .ThenInclude(f => f.Responsaveis)
            .Include(r => r.Usuario)
            .FirstOrDefaultAsync(r => r.IdRegistro == id);
        if (r is null) return null;

        return new RegistroColetaResponseDto
        {
            IdRegistro          = r.IdRegistro,
            IdFamilia           = r.IdFamilia,
            IdAluno             = r.IdAluno,
            NomeAluno           = r.Aluno != null ? r.Aluno.NomeAluno : "",
            DataNascimento      = r.Aluno != null ? r.Aluno.DataNascimento : null,
            Sexo                = r.Aluno != null ? r.Aluno.Sexo : "",
            CpfAluno            = r.Aluno != null ? r.Aluno.CpfAluno : "",
            
            NomeResponsavel       = r.Familia != null && r.Familia.Responsaveis.Any() ? r.Familia.Responsaveis.First().NomeResponsavel : "",
            ParentescoResponsavel = r.Familia != null && r.Familia.Responsaveis.Any() ? r.Familia.Responsaveis.First().ParentescoResponsavel : "",
            CpfResponsavel        = r.Familia != null && r.Familia.Responsaveis.Any() ? r.Familia.Responsaveis.First().CpfResponsavel : "",
            TelefoneResponsavel   = r.Familia != null && r.Familia.Responsaveis.Any() ? r.Familia.Responsaveis.First().TelefoneResponsavel : "",
            EmailResponsavel      = r.Familia != null && r.Familia.Responsaveis.Any() ? r.Familia.Responsaveis.First().EmailResponsavel : "",

            Endereco            = r.Familia != null ? r.Familia.Endereco : "",
            Bairro              = r.Familia != null ? r.Familia.Bairro : "",
            Comunidade          = r.Familia != null ? r.Familia.Comunidade : "",
            QtdMoradores        = r.Familia != null ? r.Familia.QtdMoradores : 0,
            RendaFamiliarMensal = r.Familia != null ? r.Familia.RendaFamiliarMensal : 0,
            RecebeBeneficioSocial = r.Familia != null && r.Familia.RecebeBeneficioSocial,
            BeneficioSocial     = r.Familia != null ? r.Familia.BeneficioSocial : null,
            PossuiInternetCasa  = r.Familia != null && r.Familia.PossuiInternetCasa,
            TipoAcessoInternet  = r.Familia != null ? r.Familia.TipoAcessoInternet : null,

            MeioTransporteEscola = r.Aluno != null && r.Aluno.Matriculas.Any() ? r.Aluno.Matriculas.First().MeioTransporteEscola : "",
            TempoDeslocamentoMin = r.Aluno != null && r.Aluno.Matriculas.Any() ? r.Aluno.Matriculas.First().TempoDeslocamentoMin : 0,
            FrequenciaEscolarPct = r.Aluno != null && r.Aluno.Matriculas.Any() ? r.Aluno.Matriculas.First().FrequenciaEscolarPct : 0,
            AnoSerie             = r.Aluno != null && r.Aluno.Matriculas.Any() ? r.Aluno.Matriculas.First().AnoSerie : "",
            Turno                = r.Aluno != null && r.Aluno.Matriculas.Any() ? r.Aluno.Matriculas.First().Turno : "",

            NecessidadeEducacionalEspecial = r.Aluno != null && r.Aluno.NecessidadeEducacionalEspecial,
            DescricaoNecessidade = r.Aluno != null ? r.Aluno.DescricaoNecessidade : null,

            CodigoFamilia       = r.Familia != null ? r.Familia.CodigoFamilia : "",
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
            var bairroTerm = bairro.ToLower();
            familiasQuery = familiasQuery.Where(f => f.Bairro.ToLower().Contains(bairroTerm));
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
