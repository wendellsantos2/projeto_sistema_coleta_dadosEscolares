using System;
using System.Collections.Generic;

namespace Application.DTOs;

// ── Familia Request/Response ──────────────────────────────────────────────────
public class FamiliaCreateDto
{
    public string CodigoFamilia { get; set; } = string.Empty;
    public string Endereco { get; set; } = string.Empty;
    public string Bairro { get; set; } = string.Empty;
    public string Comunidade { get; set; } = string.Empty;
    public int QtdMoradores { get; set; }
    public decimal RendaFamiliarMensal { get; set; }
    public bool RecebeBeneficioSocial { get; set; }
    public string? BeneficioSocial { get; set; }
    public bool PossuiInternetCasa { get; set; }
    public string? TipoAcessoInternet { get; set; }
}

public class FamiliaUpdateDto
{
    public string Endereco { get; set; } = string.Empty;
    public string Bairro { get; set; } = string.Empty;
    public string Comunidade { get; set; } = string.Empty;
    public int QtdMoradores { get; set; }
    public decimal RendaFamiliarMensal { get; set; }
    public bool RecebeBeneficioSocial { get; set; }
    public string? BeneficioSocial { get; set; }
    public bool PossuiInternetCasa { get; set; }
    public string? TipoAcessoInternet { get; set; }
}

public class FamiliaResponseDto
{
    public Guid IdFamilia { get; set; }
    public string CodigoFamilia { get; set; } = string.Empty;
    public string Endereco { get; set; } = string.Empty;
    public string Bairro { get; set; } = string.Empty;
    public string Comunidade { get; set; } = string.Empty;
    public int QtdMoradores { get; set; }
    public decimal RendaFamiliarMensal { get; set; }
    public bool RecebeBeneficioSocial { get; set; }
    public string? BeneficioSocial { get; set; }
    public bool PossuiInternetCasa { get; set; }
    public string? TipoAcessoInternet { get; set; }
    public string CriadoPorNome { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public List<AlunoResponseDto> Alunos { get; set; } = new();
}
