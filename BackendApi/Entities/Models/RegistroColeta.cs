using System;

namespace Entities.Models;

public class RegistroColeta
{
    public Guid IdRegistro { get; set; }
    public Guid IdAluno { get; set; }
    public DateTime DataColeta { get; set; }
    public string Observacao { get; set; }
    public string StatusSincronizacao { get; set; } = "SINCRONIZADO";

    public Aluno Aluno { get; set; }
}
