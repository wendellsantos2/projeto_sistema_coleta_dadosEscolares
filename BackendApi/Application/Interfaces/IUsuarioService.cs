using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.DTOs;

namespace Application.Interfaces;

public interface IUsuarioService
{
    Task<IEnumerable<UsuarioResponseDto>> ObterTodosAsync();
    Task<UsuarioResponseDto?> ObterPorIdAsync(Guid id);
    Task<UsuarioResponseDto?> AtualizarAsync(Guid id, UsuarioUpdateDto dto);
    Task<bool> ExcluirAsync(Guid id);
    Task<UsuarioResponseDto?> ObterPerfilAsync(Guid id);
}
