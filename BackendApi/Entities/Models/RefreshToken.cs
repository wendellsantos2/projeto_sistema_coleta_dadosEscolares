using System;

namespace Entities.Models;

/// <summary>
/// Armazena o hash do Refresh Token do usuário para suporte a renovação e revogação de sessão JWT.
/// </summary>
public class RefreshToken
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid IdUsuario { get; set; }

    /// <summary>
    /// Hash do token (nunca armazenar o token em texto plano).
    /// </summary>
    public string Token { get; set; } = string.Empty;

    public DateTime ExpiraEm { get; set; }
    public bool Revogado { get; set; } = false;
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// User-Agent do dispositivo/browser que gerou o token (para auditoria).
    /// </summary>
    public string? UserAgent { get; set; }

    // Navegação
    public Usuario Usuario { get; set; } = null!;
}
