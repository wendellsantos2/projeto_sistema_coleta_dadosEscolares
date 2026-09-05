using System;
using System.Collections.Generic;

namespace Entities.Models;

public class Familia
{
    public Guid IdFamilia { get; set; } = Guid.NewGuid();
    public string Endereco { get; set; }
    public string Bairro { get; set; }
    public string Comunidade { get; set; }
    public int QtdMoradores { get; set; }
    public decimal RendaFamiliarMensal { get; set; }
    public bool RecebeBeneficioSocial { get; set; }
    public string BeneficioSocial { get; set; }
    public bool PossuiInternetCasa { get; set; }
    public string TipoAcessoInternet { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Aluno> Alunos { get; set; }
}
