using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.DTOs;

namespace Application.Interfaces;

public interface IRegistroColetaService
{
    Task<IEnumerable<RegistroColetaResponseDto>> ObterTodosAsync(string? status = null);
    Task<RegistroColetaResponseDto?> ObterPorIdAsync(Guid id);
    Task<DashboardDto> ObterDashboardAsync();
}
