using System;

namespace Application.DTOs;

public class RegistroColetaResponseDto
{
    public Guid IdRegistro { get; set; }
    public Guid IdAluno { get; set; }
    public string NomeAluno { get; set; } = string.Empty;
    public string CpfAluno { get; set; } = string.Empty;
    public DateTime? DataNascimento { get; set; }
    public bool NecessidadeEducacionalEspecial { get; set; }
    
    public Guid? IdFamilia { get; set; }
    public string CodigoFamilia { get; set; } = string.Empty;
    public string Endereco { get; set; } = string.Empty;
    public string Bairro { get; set; } = string.Empty;
    public string Comunidade { get; set; } = string.Empty;
    public bool PossuiInternetCasa { get; set; }
    public string? TipoAcessoInternet { get; set; }
    public bool RecebeBeneficioSocial { get; set; }
    public string? BeneficioSocial { get; set; }
    public decimal RendaFamiliarMensal { get; set; }

    public string PesquisadorNome { get; set; } = string.Empty;
    public string? Observacao { get; set; }
    public string StatusSincronizacao { get; set; } = string.Empty;
    public DateTime DataColeta { get; set; }
    public DateTime? SincronizadoEm { get; set; }
}

public class DashboardDto
{
    public int TotalAlunos { get; set; }
    public int TotalFamilias { get; set; }
    public int TotalPesquisados { get; set; }
    public int AlunosComNecessidadeEspecial { get; set; }
    public decimal FrequenciaMediaGeral { get; set; }
    public int FamiliasComBeneficio { get; set; }
    public int FamiliasSemInternet { get; set; }
    public TransporteDistribuicaoDto[] DistribuicaoTransporte { get; set; } = Array.Empty<TransporteDistribuicaoDto>();
    public TurnoDistribuicaoDto[] DistribuicaoTurno { get; set; } = Array.Empty<TurnoDistribuicaoDto>();
    public BeneficioDistribuicaoDto[] DistribuicaoBeneficios { get; set; } = Array.Empty<BeneficioDistribuicaoDto>();
    public RendaDistribuicaoDto[] DistribuicaoRenda { get; set; } = Array.Empty<RendaDistribuicaoDto>();
}

public class TransporteDistribuicaoDto
{
    public string Tipo { get; set; } = string.Empty;
    public int Quantidade { get; set; }
}

public class TurnoDistribuicaoDto
{
    public string Turno { get; set; } = string.Empty;
    public int Quantidade { get; set; }
}

public class BeneficioDistribuicaoDto
{
    public string Beneficio { get; set; } = string.Empty;
    public int Quantidade { get; set; }
}

public class RendaDistribuicaoDto
{
    public string Faixa { get; set; } = string.Empty;
    public int Quantidade { get; set; }
}
