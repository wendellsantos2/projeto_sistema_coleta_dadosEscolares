using System;

namespace Entities.Models;

public class AlunoResponsavel
{
    public Guid IdAluno { get; set; }
    public Guid IdResponsavel { get; set; }
    public string ParentescoResponsavel { get; set; }

    public Aluno Aluno { get; set; }
    public Responsavel Responsavel { get; set; }
}
