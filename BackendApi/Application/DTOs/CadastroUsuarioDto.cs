namespace Application.DTOs;

public class CadastroUsuarioDto
{
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
    /// <summary>
    /// 1 = Admin, 2 = Gestor, 3+ = Pesquisador (default)
    /// </summary>
    public int TipoRole { get; set; }
}
