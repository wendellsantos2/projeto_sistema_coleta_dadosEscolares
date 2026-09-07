using System;
using System.Collections.Generic;

namespace Application.DTOs;

// ── Request ──────────────────────────────────────────────────────────────────
public class AlunoCreateDto
{
    public Guid? IdAluno { get; set; }
    public Guid IdFamilia { get; set; }
    public string NomeAluno { get; set; } = string.Empty;
    public DateTime DataNascimento { get; set; }
    public string Sexo { get; set; } = string.Empty;
    public string? CpfAluno { get; set; }
    public bool NecessidadeEducacionalEspecial { get; set; }
    public string? DescricaoNecessidade { get; set; }
}

public class AlunoUpdateDto
{
    public string NomeAluno { get; set; } = string.Empty;
    public DateTime DataNascimento { get; set; }
    public string Sexo { get; set; } = string.Empty;
    public string? CpfAluno { get; set; }
    public bool NecessidadeEducacionalEspecial { get; set; }
    public string? DescricaoNecessidade { get; set; }
}

// ── Response ─────────────────────────────────────────────────────────────────
public class AlunoResponseDto
{
    public Guid IdAluno { get; set; }
    public Guid IdFamilia { get; set; }
    public string NomeAluno { get; set; } = string.Empty;
    public DateTime DataNascimento { get; set; }
    public string Sexo { get; set; } = string.Empty;
    public string? CpfAluno { get; set; }
    public bool NecessidadeEducacionalEspecial { get; set; }
    public string? DescricaoNecessidade { get; set; }
    public DateTime CreatedAt { get; set; }

    public List<MatriculaResponseDto> Matriculas { get; set; } = new();
}
