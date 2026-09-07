using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.DTOs;

namespace Application.Interfaces;

public interface IAlunoService
{
    Task<IEnumerable<AlunoResponseDto>> ObterTodosAsync(Guid? idFamilia = null, string? nome = null);
    Task<AlunoResponseDto?> ObterPorIdAsync(Guid id);
    Task<AlunoResponseDto> CriarAsync(AlunoCreateDto dto);
    Task<AlunoResponseDto?> AtualizarAsync(Guid id, AlunoUpdateDto dto);
    Task<bool> ExcluirAsync(Guid id);
}
