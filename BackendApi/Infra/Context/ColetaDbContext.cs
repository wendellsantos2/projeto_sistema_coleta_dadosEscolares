using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Infra.Context;

public class ColetaDbContext : DbContext
{
    public ColetaDbContext(DbContextOptions<ColetaDbContext> options) : base(options) { }

    // ── DbSets ──────────────────────────────────────────────────────────────
    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<Familia> Familias { get; set; }
    public DbSet<Aluno> Alunos { get; set; }
    public DbSet<Responsavel> Responsaveis { get; set; }
    public DbSet<AlunoResponsavel> AlunosResponsaveis { get; set; }
    public DbSet<Matricula> Matriculas { get; set; }
    public DbSet<RegistroColeta> RegistrosColeta { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("uuid-ossp");

        // ── Usuario ──────────────────────────────────────────────────────────
        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.ToTable("usuarios");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id)
                  .HasColumnName("id")
                  .HasDefaultValueSql("uuid_generate_v4()");
            entity.Property(e => e.Nome)
                  .HasColumnName("nome")
                  .HasMaxLength(255)
                  .IsRequired();
            entity.Property(e => e.Email)
                  .HasColumnName("email")
                  .HasMaxLength(255)
                  .IsRequired();
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.SenhaHash)
                  .HasColumnName("senha_hash")
                  .HasMaxLength(255)
                  .IsRequired();
            entity.Property(e => e.Perfil)
                  .HasColumnName("perfil")
                  .HasMaxLength(20)
                  .HasDefaultValue("PESQUISADOR");
            entity.Property(e => e.Ativo)
                  .HasColumnName("ativo")
                  .HasDefaultValue(true);
            entity.Property(e => e.DataCriacao)
                  .HasColumnName("data_criacao")
                  .HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UltimoLogin)
                  .HasColumnName("ultimo_login")
                  .IsRequired(false);
        });

        // ── RefreshToken ─────────────────────────────────────────────────────
        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.ToTable("refresh_tokens");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id)
                  .HasColumnName("id")
                  .HasDefaultValueSql("uuid_generate_v4()");
            entity.Property(e => e.IdUsuario)
                  .HasColumnName("id_usuario")
                  .IsRequired();
            entity.Property(e => e.Token)
                  .HasColumnName("token")
                  .HasMaxLength(512)
                  .IsRequired();
            entity.HasIndex(e => e.Token).IsUnique();
            entity.Property(e => e.ExpiraEm)
                  .HasColumnName("expira_em")
                  .IsRequired();
            entity.Property(e => e.Revogado)
                  .HasColumnName("revogado")
                  .HasDefaultValue(false);
            entity.Property(e => e.CriadoEm)
                  .HasColumnName("criado_em")
                  .HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UserAgent)
                  .HasColumnName("user_agent")
                  .HasMaxLength(512)
                  .IsRequired(false);

            entity.HasOne(d => d.Usuario)
                  .WithMany(u => u.RefreshTokens)
                  .HasForeignKey(d => d.IdUsuario)
                  .OnDelete(DeleteBehavior.Cascade)
                  .HasConstraintName("fk_refreshtoken_usuario");
        });

        // ── Familia ──────────────────────────────────────────────────────────
        modelBuilder.Entity<Familia>(entity =>
        {
            entity.ToTable("familias");
            entity.HasKey(e => e.IdFamilia);
            entity.Property(e => e.IdFamilia)
                  .HasColumnName("id_familia")
                  .HasDefaultValueSql("uuid_generate_v4()");
            entity.Property(e => e.CodigoFamilia)
                  .HasColumnName("codigo_familia")
                  .HasMaxLength(20)
                  .IsRequired();
            entity.HasIndex(e => e.CodigoFamilia).IsUnique();
            entity.Property(e => e.Endereco).HasColumnName("endereco").HasMaxLength(255);
            entity.Property(e => e.Bairro).HasColumnName("bairro").HasMaxLength(100);
            entity.Property(e => e.Comunidade).HasColumnName("comunidade").HasMaxLength(100);
            entity.Property(e => e.QtdMoradores).HasColumnName("qtd_moradores");
            entity.Property(e => e.RendaFamiliarMensal)
                  .HasColumnName("renda_familiar_mensal")
                  .HasColumnType("decimal(10, 2)");
            entity.Property(e => e.RecebeBeneficioSocial).HasColumnName("recebe_beneficio_social");
            entity.Property(e => e.BeneficioSocial)
                  .HasColumnName("beneficio_social")
                  .HasMaxLength(255)
                  .IsRequired(false);
            entity.Property(e => e.PossuiInternetCasa).HasColumnName("possui_internet_casa");
            entity.Property(e => e.TipoAcessoInternet)
                  .HasColumnName("tipo_acesso_internet")
                  .HasMaxLength(100)
                  .IsRequired(false);
            entity.Property(e => e.CriadoPor).HasColumnName("criado_por");
            entity.Property(e => e.CreatedAt)
                  .HasColumnName("created_at")
                  .HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt)
                  .HasColumnName("updated_at")
                  .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.UsuarioCriador)
                  .WithMany(u => u.FamiliasCreadas)
                  .HasForeignKey(d => d.CriadoPor)
                  .OnDelete(DeleteBehavior.Restrict)
                  .HasConstraintName("fk_familia_usuario");
        });

        // ── Aluno ────────────────────────────────────────────────────────────
        modelBuilder.Entity<Aluno>(entity =>
        {
            entity.ToTable("alunos");
            entity.HasKey(e => e.IdAluno);
            entity.Property(e => e.IdAluno)
                  .HasColumnName("id_aluno")
                  .HasDefaultValueSql("uuid_generate_v4()");
            entity.Property(e => e.IdFamilia).HasColumnName("id_familia");
            entity.Property(e => e.NomeAluno)
                  .HasColumnName("nome_aluno")
                  .HasMaxLength(255)
                  .IsRequired();
            entity.Property(e => e.DataNascimento)
                  .HasColumnName("data_nascimento")
                  .HasColumnType("date");
            entity.Property(e => e.Sexo).HasColumnName("sexo").HasMaxLength(50);
            entity.Property(e => e.CpfAluno)
                  .HasColumnName("cpf_aluno")
                  .HasMaxLength(20)
                  .IsRequired(false);
            entity.HasIndex(e => e.CpfAluno).IsUnique();
            entity.Property(e => e.NecessidadeEducacionalEspecial)
                  .HasColumnName("necessidade_educacional_especial")
                  .HasDefaultValue(false);
            entity.Property(e => e.DescricaoNecessidade)
                  .HasColumnName("descricao_necessidade")
                  .HasColumnType("text")
                  .IsRequired(false);
            entity.Property(e => e.CreatedAt)
                  .HasColumnName("created_at")
                  .HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt)
                  .HasColumnName("updated_at")
                  .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.Familia)
                  .WithMany(p => p.Alunos)
                  .HasForeignKey(d => d.IdFamilia)
                  .OnDelete(DeleteBehavior.Cascade)
                  .HasConstraintName("fk_aluno_familia");
        });

        // ── Responsavel ──────────────────────────────────────────────────────
        modelBuilder.Entity<Responsavel>(entity =>
        {
            entity.ToTable("responsaveis");
            entity.HasKey(e => e.IdResponsavel);
            entity.Property(e => e.IdResponsavel)
                  .HasColumnName("id_responsavel")
                  .HasDefaultValueSql("uuid_generate_v4()");
            entity.Property(e => e.NomeResponsavel)
                  .HasColumnName("nome_responsavel")
                  .HasMaxLength(255)
                  .IsRequired();
            entity.Property(e => e.CpfResponsavel)
                  .HasColumnName("cpf_responsavel")
                  .HasMaxLength(20);
            entity.HasIndex(e => e.CpfResponsavel).IsUnique();
            entity.Property(e => e.TelefoneResponsavel)
                  .HasColumnName("telefone_responsavel")
                  .HasMaxLength(20);
            entity.Property(e => e.EmailResponsavel)
                  .HasColumnName("email_responsavel")
                  .HasMaxLength(255)
                  .IsRequired(false);
            entity.Property(e => e.CreatedAt)
                  .HasColumnName("created_at")
                  .HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        // ── AlunoResponsavel ─────────────────────────────────────────────────
        modelBuilder.Entity<AlunoResponsavel>(entity =>
        {
            entity.ToTable("aluno_responsavel");
            entity.HasKey(e => new { e.IdAluno, e.IdResponsavel });
            entity.Property(e => e.IdAluno).HasColumnName("id_aluno");
            entity.Property(e => e.IdResponsavel).HasColumnName("id_responsavel");
            entity.Property(e => e.ParentescoResponsavel)
                  .HasColumnName("parentesco_responsavel")
                  .HasMaxLength(100);

            entity.HasOne(d => d.Aluno)
                  .WithMany(p => p.Responsaveis)
                  .HasForeignKey(d => d.IdAluno)
                  .OnDelete(DeleteBehavior.Cascade)
                  .HasConstraintName("fk_ar_aluno");
            entity.HasOne(d => d.Responsavel)
                  .WithMany(p => p.Alunos)
                  .HasForeignKey(d => d.IdResponsavel)
                  .OnDelete(DeleteBehavior.Cascade)
                  .HasConstraintName("fk_ar_responsavel");
        });

        // ── Matricula ────────────────────────────────────────────────────────
        modelBuilder.Entity<Matricula>(entity =>
        {
            entity.ToTable("matriculas");
            entity.HasKey(e => e.IdMatricula);
            entity.Property(e => e.IdMatricula)
                  .HasColumnName("id_matricula")
                  .HasDefaultValueSql("uuid_generate_v4()");
            entity.Property(e => e.IdAluno).HasColumnName("id_aluno");
            entity.Property(e => e.AnoLetivo).HasColumnName("ano_letivo");
            entity.Property(e => e.AnoSerie).HasColumnName("ano_serie").HasMaxLength(50);
            entity.Property(e => e.Turno).HasColumnName("turno").HasMaxLength(50);
            entity.Property(e => e.FrequenciaEscolarPct)
                  .HasColumnName("frequencia_escolar_pct")
                  .HasColumnType("decimal(5, 2)");
            entity.Property(e => e.MeioTransporteEscola)
                  .HasColumnName("meio_transporte_escola")
                  .HasMaxLength(100);
            entity.Property(e => e.TempoDeslocamentoMin).HasColumnName("tempo_deslocamento_min");

            entity.HasOne(d => d.Aluno)
                  .WithMany(p => p.Matriculas)
                  .HasForeignKey(d => d.IdAluno)
                  .OnDelete(DeleteBehavior.Cascade)
                  .HasConstraintName("fk_matricula_aluno");
        });

        // ── RegistroColeta ───────────────────────────────────────────────────
        modelBuilder.Entity<RegistroColeta>(entity =>
        {
            entity.ToTable("registros_coleta");
            entity.HasKey(e => e.IdRegistro);
            entity.Property(e => e.IdRegistro)
                  .HasColumnName("id_registro")
                  .HasDefaultValueSql("uuid_generate_v4()");
            entity.Property(e => e.IdAluno).HasColumnName("id_aluno");
            entity.Property(e => e.IdFamilia)
                  .HasColumnName("id_familia")
                  .IsRequired(false);
            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
            entity.Property(e => e.DataColeta)
                  .HasColumnName("data_coleta")
                  .HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Observacao)
                  .HasColumnName("observacao")
                  .HasColumnType("text")
                  .IsRequired(false);
            entity.Property(e => e.StatusSincronizacao)
                  .HasColumnName("status_sincronizacao")
                  .HasMaxLength(20)
                  .HasDefaultValue("PENDENTE");
            entity.Property(e => e.SincronizadoEm)
                  .HasColumnName("sincronizado_em")
                  .IsRequired(false);

            entity.HasOne(d => d.Aluno)
                  .WithMany(p => p.RegistrosColeta)
                  .HasForeignKey(d => d.IdAluno)
                  .OnDelete(DeleteBehavior.Cascade)
                  .HasConstraintName("fk_registro_aluno");
            entity.HasOne(d => d.Familia)
                  .WithMany(p => p.RegistrosColeta)
                  .HasForeignKey(d => d.IdFamilia)
                  .OnDelete(DeleteBehavior.SetNull)
                  .HasConstraintName("fk_registro_familia")
                  .IsRequired(false);
            entity.HasOne(d => d.Usuario)
                  .WithMany(u => u.RegistrosColeta)
                  .HasForeignKey(d => d.IdUsuario)
                  .OnDelete(DeleteBehavior.Restrict)
                  .HasConstraintName("fk_registro_usuario");
        });
    }
}
