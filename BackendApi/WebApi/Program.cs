using Infra.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// ── JWT ──────────────────────────────────────────────────────────────────────
var secretKey = builder.Configuration["JwtSettings:SecretKey"]
                ?? throw new InvalidOperationException("JwtSettings:SecretKey nao configurada.");
var key = Encoding.ASCII.GetBytes(secretKey);

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme    = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey         = new SymmetricSecurityKey(key),
        ValidateIssuer           = false,
        ValidateAudience         = false,
        ClockSkew                = TimeSpan.Zero
    };
});

// ── Banco de Dados ────────────────────────────────────────────────────────────
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                       ?? throw new InvalidOperationException("Connection string 'DefaultConnection' nao encontrada.");

builder.Services.AddDbContext<ColetaDbContext>(options =>
    options.UseNpgsql(connectionString));

// ── Controllers ───────────────────────────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// ── Swagger com suporte a JWT ─────────────────────────────────────────────────
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title       = "API - Sistema de Coleta de Dados Escolares",
        Version     = "v1",
        Description = "API RESTful com autenticacao JWT e suporte offline-first. " +
                      "Use POST /api/auth/login para obter o token e clique em Authorize."
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name         = "Authorization",
        Type         = SecuritySchemeType.ApiKey,
        Scheme       = "Bearer",
        BearerFormat = "JWT",
        In           = ParameterLocation.Header,
        Description  = "Insira o token no formato: Bearer {seu_token}"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

// ── CORS (dev: permite todas as origens) ─────────────────────────────────────
builder.Services.AddCors(options =>
{
    options.AddPolicy("DevPolicy", policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

// ── Injecao de Dependencia (Application Services) ────────────────────────────
builder.Services.AddScoped<Application.Interfaces.IFamiliaService, Application.Services.FamiliaService>();
builder.Services.AddScoped<Application.Interfaces.ISyncService,    Application.Services.SyncService>();
builder.Services.AddScoped<Application.Interfaces.IAuthService,    Application.Services.AuthService>();
builder.Services.AddScoped<Application.Interfaces.IAlunoService,    Application.Services.AlunoService>();
builder.Services.AddScoped<Application.Interfaces.IMatriculaService, Application.Services.MatriculaService>();
builder.Services.AddScoped<Application.Interfaces.IRegistroColetaService, Application.Services.RegistroColetaService>();
builder.Services.AddScoped<Application.Interfaces.IUsuarioService,   Application.Services.UsuarioService>();

// ─────────────────────────────────────────────────────────────────────────────
var app = builder.Build();

// ── Aplicar Migrations automaticamente ao iniciar ────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ColetaDbContext>();
    db.Database.Migrate();
}

// ── Pipeline ──────────────────────────────────────────────────────────────────
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API Coleta Escolar v1");
    c.RoutePrefix = string.Empty; // Swagger abre em http://localhost:5000
});

app.UseCors("DevPolicy");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
