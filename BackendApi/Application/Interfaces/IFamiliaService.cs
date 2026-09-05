using System.Collections.Generic;
using System.Threading.Tasks;
using Application.DTOs;

namespace Application.Interfaces;

public interface IFamiliaService
{
    Task<IEnumerable<FamiliaDto>> ObterTodasAsync();
    Task<FamiliaDto> AdicionarAsync(FamiliaDto familiaDto);
}
