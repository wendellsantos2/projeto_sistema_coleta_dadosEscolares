using Infra.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Infra;

/// <summary>
/// Factory usada pelo CLI do EF Core (dotnet ef migrations) para instanciar
/// o DbContext sem precisar do Program.cs em execucao.
/// PORTA 5433: Docker PostgreSQL (evita conflito com postgres local na 5432).
/// </summary>
public class ColetaDbContextFactory : IDesignTimeDbContextFactory<ColetaDbContext>
{
    public ColetaDbContext CreateDbContext(string[] args)
    {
        var connString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")
            ?? "Host=127.0.0.1;Port=5433;Database=coleta_escolar;Username=postgres;Password=password";

        var optionsBuilder = new DbContextOptionsBuilder<ColetaDbContext>();
        optionsBuilder.UseNpgsql(connString, o => o.MigrationsAssembly("Infra"));

        return new ColetaDbContext(optionsBuilder.Options);
    }
}
