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

    public FamiliaService(ColetaDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<FamiliaDto>> ObterTodasAsync()
    {
        var familias = await _context.Familias.ToListAsync();
        return familias.Select(f => new FamiliaDto
        {
            IdFamilia = f.IdFamilia,
            Endereco = f.Endereco,
            Bairro = f.Bairro,
            Comunidade = f.Comunidade,
            QtdMoradores = f.QtdMoradores,
            RendaFamiliarMensal = f.RendaFamiliarMensal,
            RecebeBeneficioSocial = f.RecebeBeneficioSocial,
            BeneficioSocial = f.BeneficioSocial,
            PossuiInternetCasa = f.PossuiInternetCasa,
            TipoAcessoInternet = f.TipoAcessoInternet
        });
    }

    public async Task<FamiliaDto> AdicionarAsync(FamiliaDto dto)
    {
        var familia = new Familia
        {
            IdFamilia = dto.IdFamilia ?? Guid.NewGuid(),
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
        await _context.SaveChangesAsync();

        dto.IdFamilia = familia.IdFamilia;
        return dto;
    }
}
