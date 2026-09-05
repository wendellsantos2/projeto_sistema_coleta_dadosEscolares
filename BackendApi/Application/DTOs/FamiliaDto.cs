using System;

namespace Application.DTOs;

public class FamiliaDto
{
    public Guid? IdFamilia { get; set; }
    public string Endereco { get; set; } = string.Empty;
    public string Bairro { get; set; } = string.Empty;
    public string Comunidade { get; set; } = string.Empty;
    public int QtdMoradores { get; set; }
    public decimal RendaFamiliarMensal { get; set; }
    public bool RecebeBeneficioSocial { get; set; }
    public string BeneficioSocial { get; set; } = string.Empty;
    public bool PossuiInternetCasa { get; set; }
    public string TipoAcessoInternet { get; set; } = string.Empty;
}
