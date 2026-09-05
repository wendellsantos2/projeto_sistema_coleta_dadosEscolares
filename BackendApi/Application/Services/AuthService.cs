using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Interfaces;
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

    public async Task<TokenDto> LoginAsync(LoginDto loginDto)
    {
        // Obs: Em um cenário real, as senhas estariam usando BCrypt. 
        // Aqui estamos simplificando para o teste técnico.
        var usuario = await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Email == loginDto.Email && u.SenhaHash == loginDto.Senha);

        if (usuario == null)
            throw new UnauthorizedAccessException("Usuário ou senha inválidos.");

        var tokenHandler = new JwtSecurityTokenHandler();
        var secretKey = _configuration.GetValue<string>("JwtSettings:SecretKey") ?? "uma-chave-secreta-muito-longa-e-segura-123456";
        var key = Encoding.ASCII.GetBytes(secretKey);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, usuario.Email),
                new Claim(ClaimTypes.Role, usuario.Role)
            }),
            Expires = DateTime.UtcNow.AddHours(2),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);

        return new TokenDto
        {
            Token = tokenHandler.WriteToken(token),
            Role = usuario.Role
        };
    }
}
