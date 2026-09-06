using System;

namespace Entities.Models;

public class RegistroColeta
{
    public Guid IdRegistro { get; set; } = Guid.NewGuid();

    public Guid IdAluno { get; set; }
    public Guid? IdFamilia { get; set; }

    /// <summary>
    /// FK para o usuário (pesquisador) que realizou a coleta.
    /// </summary>
    public Guid IdUsuario { get; set; }

    public DateTime DataColeta { get; set; } = DateTime.UtcNow;
    public string? Observacao { get; set; }

    /// <summary>
    /// Status de sincronização: PENDENTE | SINCRONIZADO
    /// Usado pelo aplicativo mobile no modo offline-first.
    /// </summary>
    public string StatusSincronizacao { get; set; } = "PENDENTE";

    public DateTime? SincronizadoEm { get; set; }

    // Navegação
    public Aluno Aluno { get; set; } = null!;
    public Familia? Familia { get; set; }
    public Usuario Usuario { get; set; } = null!;
}
