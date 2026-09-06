using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infra.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:uuid-ossp", ",,");

            migrationBuilder.CreateTable(
                name: "responsaveis",
                columns: table => new
                {
                    id_responsavel = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    nome_responsavel = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    cpf_responsavel = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    telefone_responsavel = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    email_responsavel = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_responsaveis", x => x.id_responsavel);
                });

            migrationBuilder.CreateTable(
                name: "usuarios",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    nome = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    senha_hash = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    perfil = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "PESQUISADOR"),
                    ativo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    data_criacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    ultimo_login = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usuarios", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "familias",
                columns: table => new
                {
                    id_familia = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    codigo_familia = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    endereco = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    bairro = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    comunidade = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    qtd_moradores = table.Column<int>(type: "integer", nullable: false),
                    renda_familiar_mensal = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    recebe_beneficio_social = table.Column<bool>(type: "boolean", nullable: false),
                    beneficio_social = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    possui_internet_casa = table.Column<bool>(type: "boolean", nullable: false),
                    tipo_acesso_internet = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    criado_por = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_familias", x => x.id_familia);
                    table.ForeignKey(
                        name: "fk_familia_usuario",
                        column: x => x.criado_por,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "refresh_tokens",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    id_usuario = table.Column<Guid>(type: "uuid", nullable: false),
                    token = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    expira_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    revogado = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    user_agent = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_refresh_tokens", x => x.id);
                    table.ForeignKey(
                        name: "fk_refreshtoken_usuario",
                        column: x => x.id_usuario,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "alunos",
                columns: table => new
                {
                    id_aluno = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    id_familia = table.Column<Guid>(type: "uuid", nullable: false),
                    nome_aluno = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    data_nascimento = table.Column<DateTime>(type: "date", nullable: false),
                    sexo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    cpf_aluno = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    necessidade_educacional_especial = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    descricao_necessidade = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_alunos", x => x.id_aluno);
                    table.ForeignKey(
                        name: "fk_aluno_familia",
                        column: x => x.id_familia,
                        principalTable: "familias",
                        principalColumn: "id_familia",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "aluno_responsavel",
                columns: table => new
                {
                    id_aluno = table.Column<Guid>(type: "uuid", nullable: false),
                    id_responsavel = table.Column<Guid>(type: "uuid", nullable: false),
                    parentesco_responsavel = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_aluno_responsavel", x => new { x.id_aluno, x.id_responsavel });
                    table.ForeignKey(
                        name: "fk_ar_aluno",
                        column: x => x.id_aluno,
                        principalTable: "alunos",
                        principalColumn: "id_aluno",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_ar_responsavel",
                        column: x => x.id_responsavel,
                        principalTable: "responsaveis",
                        principalColumn: "id_responsavel",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "matriculas",
                columns: table => new
                {
                    id_matricula = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    id_aluno = table.Column<Guid>(type: "uuid", nullable: false),
                    ano_letivo = table.Column<int>(type: "integer", nullable: false),
                    ano_serie = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    turno = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    frequencia_escolar_pct = table.Column<decimal>(type: "numeric(5,2)", nullable: false),
                    meio_transporte_escola = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    tempo_deslocamento_min = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_matriculas", x => x.id_matricula);
                    table.ForeignKey(
                        name: "fk_matricula_aluno",
                        column: x => x.id_aluno,
                        principalTable: "alunos",
                        principalColumn: "id_aluno",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "registros_coleta",
                columns: table => new
                {
                    id_registro = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    id_aluno = table.Column<Guid>(type: "uuid", nullable: false),
                    id_familia = table.Column<Guid>(type: "uuid", nullable: true),
                    id_usuario = table.Column<Guid>(type: "uuid", nullable: false),
                    data_coleta = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    observacao = table.Column<string>(type: "text", nullable: true),
                    status_sincronizacao = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "PENDENTE"),
                    sincronizado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_registros_coleta", x => x.id_registro);
                    table.ForeignKey(
                        name: "fk_registro_aluno",
                        column: x => x.id_aluno,
                        principalTable: "alunos",
                        principalColumn: "id_aluno",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_registro_familia",
                        column: x => x.id_familia,
                        principalTable: "familias",
                        principalColumn: "id_familia",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_registro_usuario",
                        column: x => x.id_usuario,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_aluno_responsavel_id_responsavel",
                table: "aluno_responsavel",
                column: "id_responsavel");

            migrationBuilder.CreateIndex(
                name: "IX_alunos_cpf_aluno",
                table: "alunos",
                column: "cpf_aluno",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_alunos_id_familia",
                table: "alunos",
                column: "id_familia");

            migrationBuilder.CreateIndex(
                name: "IX_familias_codigo_familia",
                table: "familias",
                column: "codigo_familia",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_familias_criado_por",
                table: "familias",
                column: "criado_por");

            migrationBuilder.CreateIndex(
                name: "IX_matriculas_id_aluno",
                table: "matriculas",
                column: "id_aluno");

            migrationBuilder.CreateIndex(
                name: "IX_refresh_tokens_id_usuario",
                table: "refresh_tokens",
                column: "id_usuario");

            migrationBuilder.CreateIndex(
                name: "IX_refresh_tokens_token",
                table: "refresh_tokens",
                column: "token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_registros_coleta_id_aluno",
                table: "registros_coleta",
                column: "id_aluno");

            migrationBuilder.CreateIndex(
                name: "IX_registros_coleta_id_familia",
                table: "registros_coleta",
                column: "id_familia");

            migrationBuilder.CreateIndex(
                name: "IX_registros_coleta_id_usuario",
                table: "registros_coleta",
                column: "id_usuario");

            migrationBuilder.CreateIndex(
                name: "IX_responsaveis_cpf_responsavel",
                table: "responsaveis",
                column: "cpf_responsavel",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_email",
                table: "usuarios",
                column: "email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "aluno_responsavel");

            migrationBuilder.DropTable(
                name: "matriculas");

            migrationBuilder.DropTable(
                name: "refresh_tokens");

            migrationBuilder.DropTable(
                name: "registros_coleta");

            migrationBuilder.DropTable(
                name: "responsaveis");

            migrationBuilder.DropTable(
                name: "alunos");

            migrationBuilder.DropTable(
                name: "familias");

            migrationBuilder.DropTable(
                name: "usuarios");
        }
    }
}
