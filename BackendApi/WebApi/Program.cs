using Infra.Context;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "API Sistema de Coleta de Dados Escolares",
        Version = "v1",
        Description = "API robusta com suporte offline-first, desenvolvida para coletar e sincronizar dados socioeconômicos de alunos e suas famílias.",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Teste Técnico",
            Email = "candidato@teste.com"
        }
    });
});

// Configure DbContext
builder.Services.AddDbContext<ColetaDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => 
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API Coleta Escolar v1");
        c.RoutePrefix = string.Empty; // Abre o Swagger na raiz (localhost)
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
