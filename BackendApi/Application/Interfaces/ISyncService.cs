using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.DTOs;

namespace Application.Interfaces;

public interface ISyncService
{
    Task SincronizarLoteAsync(IEnumerable<ColetaSyncDto> coletas, Guid idUsuario);
}
