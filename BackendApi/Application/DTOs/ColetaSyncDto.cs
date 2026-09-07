using System;

namespace Application.DTOs;

public class ColetaSyncDto
{
    public Guid IdRegistro { get; set; }
    public Guid IdFamilia { get; set; }
    public Guid IdAluno { get; set; }
    
    // Aluno
    public string NomeAluno { get; set; }
    public DateTime DataNascimento { get; set; }
    public string Sexo { get; set; }
    public string? CpfAluno { get; set; }
    public bool NecessidadeEducacionalEspecial { get; set; }
    public string? DescricaoNecessidade { get; set; }
    
    // Responsavel
    public string NomeResponsavel { get; set; }
    public string ParentescoResponsavel { get; set; }
    public string CpfResponsavel { get; set; }
    public string TelefoneResponsavel { get; set; }
    public string? EmailResponsavel { get; set; }
    
    // Familia
    public string Endereco { get; set; }
    public string Bairro { get; set; }
    public string Comunidade { get; set; }
    public int QtdMoradores { get; set; }
    public decimal RendaFamiliarMensal { get; set; }
    public bool RecebeBeneficioSocial { get; set; }
    public string? BeneficioSocial { get; set; }
    public bool PossuiInternetCasa { get; set; }
    public string? TipoAcessoInternet { get; set; }
    
    // Matricula
    public string MeioTransporteEscola { get; set; }
    public int TempoDeslocamentoMin { get; set; }
    public decimal FrequenciaEscolarPct { get; set; }
    public string AnoSerie { get; set; }
    public string Turno { get; set; }
    
    // Registro
    public string? Observacao { get; set; }
}
