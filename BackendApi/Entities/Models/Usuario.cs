using System;
using System.Collections.Generic;

namespace Entities.Models;

public class Usuario
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string SenhaHash { get; set; } = string.Empty;

    /// <summary>
    /// Perfil de acesso: PESQUISADOR | GESTOR | ADMIN
    /// </summary>
    public string Perfil { get; set; } = "PESQUISADOR";

    public bool Ativo { get; set; } = true;
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
    public DateTime? UltimoLogin { get; set; }

    // Navegação
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    public ICollection<Familia> FamiliasCreadas { get; set; } = new List<Familia>();
    public ICollection<RegistroColeta> RegistrosColeta { get; set; } = new List<RegistroColeta>();
}
