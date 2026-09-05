using System;
using System.Collections.Generic;

namespace Entities.Models;

public class Responsavel
{
    public Guid IdResponsavel { get; set; } = Guid.NewGuid();
    public string NomeResponsavel { get; set; }
    public string CpfResponsavel { get; set; }
    public string TelefoneResponsavel { get; set; }
    public string EmailResponsavel { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<AlunoResponsavel> Alunos { get; set; }
}
