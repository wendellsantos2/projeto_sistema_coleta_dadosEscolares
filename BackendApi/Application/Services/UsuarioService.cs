using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Interfaces;
using Infra.Context;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class UsuarioService : IUsuarioService
{
    private readonly ColetaDbContext _context;
    public UsuarioService(ColetaDbContext context) => _context = context;

    public async Task<IEnumerable<UsuarioResponseDto>> ObterTodosAsync()
    {
        var usuarios = await _context.Usuarios.ToListAsync();
        return usuarios.Select(MapToResponse);
    }

    public async Task<UsuarioResponseDto?> ObterPorIdAsync(Guid id)
    {
        var u = await _context.Usuarios.FindAsync(id);
        return u is null ? null : MapToResponse(u);
    }

    public async Task<UsuarioResponseDto?> ObterPerfilAsync(Guid id) => await ObterPorIdAsync(id);

    public async Task<UsuarioResponseDto?> AtualizarAsync(Guid id, UsuarioUpdateDto dto)
    {
        var u = await _context.Usuarios.FindAsync(id);
        if (u is null) return null;
        u.Nome  = dto.Nome;
        u.Perfil = dto.Perfil;
        u.Ativo = dto.Ativo;
        await _context.SaveChangesAsync();
        return MapToResponse(u);
    }

    public async Task<bool> ExcluirAsync(Guid id)
    {
        var u = await _context.Usuarios.FindAsync(id);
        if (u is null) return false;
        _context.Usuarios.Remove(u);
        await _context.SaveChangesAsync();
        return true;
    }

    private static UsuarioResponseDto MapToResponse(Entities.Models.Usuario u) => new()
    {
        Id          = u.Id,
        Nome        = u.Nome,
        Email       = u.Email,
        Perfil      = u.Perfil,
        Ativo       = u.Ativo,
        DataCriacao = u.DataCriacao,
        UltimoLogin = u.UltimoLogin
    };
}
