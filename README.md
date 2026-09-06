# Sistema de Coleta de Dados Escolares

Solucao completa para coleta, armazenamento, sincronizacao e visualizacao de dados
socioeconomicos de alunos e suas familias, desenvolvida como teste tecnico.

---

## 1. Arquitetura

```
[Mobile Flutter] --offline-first--> [API .NET 9 / C#] --> [PostgreSQL 15]
                                           |
                              [Web Dashboard Angular 21]
```

- **Mobile (Flutter):** Coleta dados em campo. Opera offline com SQLite local e sincroniza com a API quando ha conexao.
- **Backend (ASP.NET Core + EF Core):** API RESTful com autenticacao JWT, arquitetura em camadas (DDD).
- **Banco de Dados (PostgreSQL 15):** Gerenciado via Docker e migrations do Entity Framework Core.
- **Web Dashboard (Angular 21):** Painel SPA para visualizacao de indicadores e consulta de registros.

---

## 2. Tecnologias Utilizadas

| Componente        | Tecnologia                          |
|-------------------|-------------------------------------|
| Backend / API     | .NET 9 (C#), ASP.NET Core, EF Core  |
| Banco de Dados    | PostgreSQL 15 (Docker)              |
| ORM / Migrations  | Entity Framework Core 8 + Npgsql    |
| Autenticacao      | JWT Bearer (System.IdentityModel)   |
| Mobile            | Flutter (Dart) + SQLite             |
| Web Dashboard     | Angular 21 + Chart.js               |
| Infraestrutura    | Docker, Docker Compose              |

---

## 3. Estrutura do Projeto

```
/
├── BackendApi/               # Solucao .NET (DDD)
│   ├── WebApi/               # Controllers, Program.cs, appsettings.json
│   ├── Application/          # Services, DTOs, Interfaces (casos de uso)
│   ├── Domain/               # Regras de negocio puras
│   ├── Entities/             # Entidades / Models (POCOs)
│   │   └── Models/           # Aluno, Familia, Usuario, RefreshToken, etc.
│   └── Infra/                # DbContext, Migrations, ColetaDbContextFactory
├── mobile-app/               # Projeto Flutter (offline-first)
├── web-dashboard/            # Projeto Angular 21
├── database/
│   └── init.sql              # Apenas cria extensao uuid-ossp (tabelas via migrations)
├── docs/                     # Documentacao: MER, Casos de Uso
└── docker-compose.yml        # Sobe o PostgreSQL na porta 5433
```

---

## 4. Configuracao do Banco de Dados

### Pre-requisitos
- Docker Desktop instalado e em execucao
- Porta **5433** livre (o PostgreSQL local geralmente ocupa a 5432)

> **Por que porta 5433?**
> Se voce tiver o PostgreSQL instalado localmente no Windows, ele ja ocupa a porta 5432.
> O Docker foi configurado para mapear a porta **5433** do host para a 5432 do container,
> evitando conflito de porta.

### Subir o banco de dados

```bash
# Na raiz do projeto:
docker-compose up -d
```

O container `coleta_escolar_db` ira iniciar com:
- **Host:** 127.0.0.1
- **Porta:** 5433
- **Banco:** coleta_escolar
- **Usuario:** postgres
- **Senha:** password

### Aplicar as Migrations (criar as tabelas)

```bash
dotnet ef database update \
  --project BackendApi/Infra/Infra.csproj \
  --startup-project BackendApi/Infra/Infra.csproj
```

Isso criara automaticamente todas as 9 tabelas:
`usuarios`, `refresh_tokens`, `familias`, `alunos`, `responsaveis`,
`aluno_responsavel`, `matriculas`, `registros_coleta`, `__EFMigrationsHistory`

---

## 5. Execucao do Backend (.NET 9)

### Pre-requisitos
- .NET 9 SDK instalado (`dotnet --version` deve retornar 9.x)
- Banco de dados rodando (passo anterior)

### Configuracao

O arquivo `BackendApi/WebApi/appsettings.json` ja vem pre-configurado:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=127.0.0.1;Port=5433;Database=coleta_escolar;Username=postgres;Password=password"
  },
  "JwtSettings": {
    "SecretKey": "coleta-escolar-chave-secreta-jwt-2026-muito-longa-e-segura"
  }
}
```

> Em producao, substitua `SecretKey` por uma chave forte e armazene em variaveis de ambiente
> ou em um gerenciador de segredos (Azure Key Vault, AWS Secrets Manager, etc.).

### Iniciar a API

```bash
dotnet run --project BackendApi/WebApi/WebApi.csproj
```

A API estara disponivel em:
- **Swagger UI:** http://localhost:5000 (abre automaticamente na raiz)
- **API Base URL:** http://localhost:5000/api

### Endpoints principais

| Metodo | Rota                  | Auth | Descricao                       |
|--------|-----------------------|------|---------------------------------|
| POST   | /api/auth/cadastrar   | Nao  | Cadastra novo usuario           |
| POST   | /api/auth/login       | Nao  | Autentica e retorna JWT         |
| POST   | /api/sync             | Sim  | Sincroniza coletas do mobile    |
| GET    | /api/familias         | Sim  | Lista familias cadastradas      |

---

## 6. Execucao do Mobile (Flutter)

### Pre-requisitos
- Flutter SDK >= 3.x instalado
- Emulador Android/iOS ou dispositivo fisico conectado
- API do backend rodando localmente

### Instalar dependencias e rodar

```bash
cd mobile-app
flutter pub get
flutter run
```

### Testar modo offline

1. Inicie o app com internet ativa e faca login.
2. Desligue o Wi-Fi/dados moveis.
3. Cadastre novos registros — eles ficam salvos localmente com status `PENDENTE`.
4. Reative a internet e pressione "Sincronizar" para enviar ao backend.

---

## 7. Execucao do Web Dashboard (Angular 21)

### Pre-requisitos
- Node.js >= 20 e npm >= 10 instalados
- API do backend rodando localmente

### Instalar dependencias e rodar

```bash
cd web-dashboard
npm install
npm start
```

Acesse **http://localhost:4200** no navegador.

---

## 8. Fluxo Completo (Passo a Passo)

```bash
# 1. Sobe o banco
docker-compose up -d

# 2. Aplica as migrations
dotnet ef database update \
  --project BackendApi/Infra/Infra.csproj \
  --startup-project BackendApi/Infra/Infra.csproj

# 3. Inicia a API (terminal separado)
dotnet run --project BackendApi/WebApi/WebApi.csproj

# 4. Inicia o Web Dashboard (terminal separado)
cd web-dashboard && npm install && npm start

# 5. Inicia o Mobile (terminal separado)
cd mobile-app && flutter pub get && flutter run
```

---

## 9. Decisoes Tecnicas

| Decisao | Justificativa |
|---------|---------------|
| **UUID como PK em RegistroColeta** | Permite gerar IDs no dispositivo mobile offline sem risco de colisão ao sincronizar. |
| **EF Core Migrations** | Controle de versao do schema do banco — evita scripts SQL manuais desatualizados. |
| **Docker na porta 5433** | Evita conflito com PostgreSQL local do Windows que ocupa a 5432. |
| **JWT com Perfis (RBAC)** | `PESQUISADOR`, `GESTOR`, `ADMIN` — controle de acesso simples e extensivel. |
| **init.sql minimo** | O schema e gerenciado inteiramente pelo EF. O `init.sql` apenas cria a extensao `uuid-ossp`. |
| **Campo `criado_por` em Familia** | Rastreabilidade — saber qual pesquisador cadastrou cada familia. |
| **`sincronizado_em` em RegistroColeta** | Permite ao app mobile identificar registros pendentes de sync. |

---

## 10. Pendencias e Proximos Passos

- [ ] Implementar rotacao de Refresh Token (endpoint `POST /auth/refresh`)
- [ ] Hash de senha com BCrypt (atualmente armazenado em texto plano para fins de teste)
- [ ] Testes unitarios nas camadas Application e Domain
- [ ] CORS configurado por ambiente (dev vs producao)
- [ ] Deploy em nuvem (Railway, Render, Azure App Service)
