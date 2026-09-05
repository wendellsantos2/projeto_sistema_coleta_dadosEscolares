using System.Threading.Tasks;
using Application.DTOs;

namespace Application.Interfaces;

public interface IAuthService
{
    Task<TokenDto> LoginAsync(LoginDto loginDto);
    Task CadastrarAsync(CadastroUsuarioDto cadastroDto);
}
