using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
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
        var usuarioExistente = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == cadastroDto.Email);
        if (usuarioExistente != null)
            throw new Exception("Já existe um usuário com este e-mail.");

        string roleName = cadastroDto.TipoRole == 1 ? "Admin" : "Coletor";

        var usuario = new Usuario
        {
            Id = Guid.NewGuid(),
            Email = cadastroDto.Email,
            SenhaHash = cadastroDto.Senha, // Num cenário real, deve-se aplicar Hash (ex: BCrypt)
            Role = roleName
        };

        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();
    }

    public async Task<TokenDto> LoginAsync(LoginDto loginDto)
    {
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
