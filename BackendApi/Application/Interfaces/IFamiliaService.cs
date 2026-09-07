using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.DTOs;

namespace Application.Interfaces;

public interface IFamiliaService
{
    Task<IEnumerable<FamiliaResponseDto>> ObterTodasAsync(string? bairro = null, string? comunidade = null);
    Task<FamiliaResponseDto?> ObterPorIdAsync(Guid id);
    Task<FamiliaResponseDto> CriarAsync(FamiliaCreateDto dto, Guid idUsuario);
    Task<FamiliaResponseDto?> AtualizarAsync(Guid id, FamiliaUpdateDto dto);
    Task<bool> ExcluirAsync(Guid id);
}
