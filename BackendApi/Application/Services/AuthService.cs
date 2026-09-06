using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Interfaces;
using Entities.Models;
using Infra.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Application.Services;

public class AuthService : IAuthService
{
    private readonly ColetaDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthService(ColetaDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task CadastrarAsync(CadastroUsuarioDto cadastroDto)
    {
        var usuarioExistente = await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Email == cadastroDto.Email);

        if (usuarioExistente != null)
            throw new Exception("Ja existe um usuario com este e-mail.");

        string perfil = cadastroDto.TipoRole switch
        {
            1 => "ADMIN",
            2 => "GESTOR",
            _ => "PESQUISADOR"
        };

        var usuario = new Usuario
        {
            Id = Guid.NewGuid(),
            Nome = cadastroDto.Nome ?? cadastroDto.Email,
            Email = cadastroDto.Email,
            SenhaHash = cadastroDto.Senha,
            Perfil = perfil,
            Ativo = true
        };

        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();
    }

    public async Task<TokenDto> LoginAsync(LoginDto loginDto)
    {
        var usuario = await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Email == loginDto.Email && u.SenhaHash == loginDto.Senha);

        if (usuario == null)
            throw new UnauthorizedAccessException("Usuario ou senha invalidos.");

        if (!usuario.Ativo)
            throw new UnauthorizedAccessException("Conta desativada. Contate o administrador.");

        usuario.UltimoLogin = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        var secretKey = _configuration.GetValue<string>("JwtSettings:SecretKey")
                        ?? "uma-chave-secreta-muito-longa-e-segura-123456";
        var key = Encoding.ASCII.GetBytes(secretKey);

        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Email, usuario.Email),
                new Claim(ClaimTypes.Name, usuario.Nome),
                new Claim(ClaimTypes.Role, usuario.Perfil)
            }),
            Expires = DateTime.UtcNow.AddHours(2),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);

        return new TokenDto
        {
            Token = tokenHandler.WriteToken(token),
            Role = usuario.Perfil
        };
    }
}
