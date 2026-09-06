using System;
using System.Collections.Generic;

namespace Entities.Models;

public class Familia
{
    public Guid IdFamilia { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Código legível da família, ex: FAM-001.
    /// </summary>
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

    /// <summary>
    /// FK para o usuário (pesquisador) que cadastrou a família.
    /// </summary>
    public Guid CriadoPor { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navegação
    public Usuario UsuarioCriador { get; set; } = null!;
    public ICollection<Aluno> Alunos { get; set; } = new List<Aluno>();
    public ICollection<RegistroColeta> RegistrosColeta { get; set; } = new List<RegistroColeta>();
}
