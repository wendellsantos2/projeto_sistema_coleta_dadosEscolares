using System;
using System.Collections.Generic;

namespace Entities.Models;

public class Aluno
{
    public Guid IdAluno { get; set; } = Guid.NewGuid();
    public Guid IdFamilia { get; set; }
    public string NomeAluno { get; set; }
    public DateTime DataNascimento { get; set; }
    public string Sexo { get; set; }
    public string CpfAluno { get; set; }
    public bool NecessidadeEducacionalEspecial { get; set; }
    public string DescricaoNecessidade { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public Familia Familia { get; set; }
    public ICollection<AlunoResponsavel> Responsaveis { get; set; }
    public ICollection<Matricula> Matriculas { get; set; }
    public ICollection<RegistroColeta> RegistrosColeta { get; set; }
}
