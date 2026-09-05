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

public class SyncService : ISyncService
{
    private readonly ColetaDbContext _context;

    public SyncService(ColetaDbContext context)
    {
        _context = context;
    }

    public async Task SincronizarLoteAsync(IEnumerable<ColetaSyncDto> coletas)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            foreach (var dto in coletas)
            {
                // Verifica se já sincronizou
                if (await _context.RegistrosColeta.AnyAsync(r => r.IdRegistro == dto.IdRegistro))
                    continue;

                // 1. Processar Família
                var familia = await _context.Familias.FirstOrDefaultAsync(f => f.IdFamilia == dto.IdFamilia);
                if (familia == null)
                {
                    familia = new Familia
                    {
                        IdFamilia = dto.IdFamilia,
                        Endereco = dto.Endereco,
                        Bairro = dto.Bairro,
                        Comunidade = dto.Comunidade,
                        QtdMoradores = dto.QtdMoradores,
                        RendaFamiliarMensal = dto.RendaFamiliarMensal,
                        RecebeBeneficioSocial = dto.RecebeBeneficioSocial,
                        BeneficioSocial = dto.BeneficioSocial,
                        PossuiInternetCasa = dto.PossuiInternetCasa,
                        TipoAcessoInternet = dto.TipoAcessoInternet,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };
                    _context.Familias.Add(familia);
                }

                // 2. Processar Aluno
                var aluno = await _context.Alunos.FirstOrDefaultAsync(a => a.IdAluno == dto.IdAluno);
                if (aluno == null)
                {
                    aluno = new Aluno
                    {
                        IdAluno = dto.IdAluno,
                        IdFamilia = familia.IdFamilia,
                        NomeAluno = dto.NomeAluno,
                        DataNascimento = dto.DataNascimento,
                        Sexo = dto.Sexo,
                        CpfAluno = dto.CpfAluno,
                        NecessidadeEducacionalEspecial = dto.NecessidadeEducacionalEspecial,
                        DescricaoNecessidade = dto.DescricaoNecessidade,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };
                    _context.Alunos.Add(aluno);
                }

                // 3. Processar Responsável
                var responsavel = await _context.Responsaveis.FirstOrDefaultAsync(r => r.CpfResponsavel == dto.CpfResponsavel);
                if (responsavel == null)
                {
                    responsavel = new Responsavel
                    {
                        IdResponsavel = Guid.NewGuid(),
                        NomeResponsavel = dto.NomeResponsavel,
                        CpfResponsavel = dto.CpfResponsavel,
                        TelefoneResponsavel = dto.TelefoneResponsavel,
                        EmailResponsavel = dto.EmailResponsavel,
                        CreatedAt = DateTime.UtcNow
                    };
                    _context.Responsaveis.Add(responsavel);
                }

                // 4. Relacionar Aluno e Responsável
                bool relExiste = await _context.AlunosResponsaveis.AnyAsync(ar => ar.IdAluno == aluno.IdAluno && ar.IdResponsavel == responsavel.IdResponsavel);
                if (!relExiste)
                {
                    _context.AlunosResponsaveis.Add(new AlunoResponsavel
                    {
                        IdAluno = aluno.IdAluno,
                        IdResponsavel = responsavel.IdResponsavel,
                        ParentescoResponsavel = dto.ParentescoResponsavel
                    });
                }

                // 5. Matrícula
                _context.Matriculas.Add(new Matricula
                {
                    IdMatricula = Guid.NewGuid(),
                    IdAluno = aluno.IdAluno,
                    AnoLetivo = DateTime.UtcNow.Year,
                    AnoSerie = dto.AnoSerie,
                    Turno = dto.Turno,
                    FrequenciaEscolarPct = dto.FrequenciaEscolarPct,
                    MeioTransporteEscola = dto.MeioTransporteEscola,
                    TempoDeslocamentoMin = dto.TempoDeslocamentoMin
                });

                // 6. Registro de Coleta
                _context.RegistrosColeta.Add(new RegistroColeta
                {
                    IdRegistro = dto.IdRegistro,
                    IdAluno = aluno.IdAluno,
                    DataColeta = DateTime.UtcNow,
                    Observacao = dto.Observacao,
                    StatusSincronizacao = "SINCRONIZADO"
                });

                // Salva o lote de inserções desse registro
                await _context.SaveChangesAsync();
            }

            await transaction.CommitAsync();
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}
