namespace Application.DTOs;

public class CadastroUsuarioDto
{
    public string Email { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
    /// <summary>
    /// 1 = Admin, 2 = Coletor (Usuário)
    /// </summary>
    public int TipoRole { get; set; } 
}
