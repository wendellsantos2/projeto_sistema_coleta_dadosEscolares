using System;

namespace Application.DTOs;

// ── Request ──────────────────────────────────────────────────────────────────
public class MatriculaCreateDto
{
    public Guid IdAluno { get; set; }
    public int AnoLetivo { get; set; }
    public string AnoSerie { get; set; } = string.Empty;
    public string Turno { get; set; } = string.Empty;
    public decimal FrequenciaEscolarPct { get; set; }
    public string MeioTransporteEscola { get; set; } = string.Empty;
    public int TempoDeslocamentoMin { get; set; }
}

// ── Response ─────────────────────────────────────────────────────────────────
public class MatriculaResponseDto
{
    public Guid IdMatricula { get; set; }
    public Guid IdAluno { get; set; }
    public int AnoLetivo { get; set; }
    public string AnoSerie { get; set; } = string.Empty;
    public string Turno { get; set; } = string.Empty;
    public decimal FrequenciaEscolarPct { get; set; }
    public string MeioTransporteEscola { get; set; } = string.Empty;
    public int TempoDeslocamentoMin { get; set; }
}
