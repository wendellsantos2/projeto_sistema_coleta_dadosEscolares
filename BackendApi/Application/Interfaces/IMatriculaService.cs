using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.DTOs;

namespace Application.Interfaces;

public interface IMatriculaService
{
    Task<IEnumerable<MatriculaResponseDto>> ObterPorAlunoAsync(Guid idAluno);
    Task<MatriculaResponseDto> CriarAsync(MatriculaCreateDto dto);
}
