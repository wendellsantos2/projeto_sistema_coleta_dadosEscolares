using System;

namespace Entities.Models;

public class Matricula
{
    public Guid IdMatricula { get; set; } = Guid.NewGuid();
    public Guid IdAluno { get; set; }
    public int AnoLetivo { get; set; }
    public string AnoSerie { get; set; }
    public string Turno { get; set; }
    public decimal FrequenciaEscolarPct { get; set; }
    public string MeioTransporteEscola { get; set; }
    public int TempoDeslocamentoMin { get; set; }

    public Aluno Aluno { get; set; }
}
