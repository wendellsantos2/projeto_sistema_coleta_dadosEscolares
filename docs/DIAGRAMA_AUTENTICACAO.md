# Diagrama de Caso de Uso: Autenticação e Perfis (Admin vs Coletor)

Este documento ilustra a separação de responsabilidades caso o sistema de Autenticação (Login) com perfis de acesso seja implementado.

## Diagrama de Atores e Fluxos

Abaixo, o diagrama visual representando o fluxo de segurança utilizando JWT (JSON Web Tokens):

```mermaid
%%{init: {'theme': 'base', 'themeVariables': { 'primaryColor': '#e0f7fa', 'edgeLabelBackground':'#ffffff'}}}%%
flowchart TD
    %% Atores
    Coletor([🧑‍💻 Usuário Coletor\nApp Mobile])
    Admin([👔 Administrador\nPainel Angular])

    %% Backend / Autenticação
    Auth((Serviço de Autenticação\nAPI .NET))
    DB[(Banco de Dados\nPostgreSQL)]

    %% Funcionalidades Coletor
    F1[Login Celular]
    F2[Coletar Dados Offline]
    F3[Sincronizar Lote Pendente]

    %% Funcionalidades Admin
    F4[Login Web]
    F5[Visualizar Dashboard]
    F6[Gerenciar Usuários e Coletas]

    %% Relações Coletor
    Coletor -->|1. Informa e-mail/senha| F1
    F1 -->|Valida| Auth
    Auth -.->|Devolve Token 'Coletor'| Coletor
    
    Coletor -->|2. Preenche Fichas| F2
    F2 -->|Salva no SQLite Local| F2
    Coletor -->|3. Ao ter Internet| F3
    F3 -- Requer Token 'Coletor' --> Auth
    Auth -->|Acesso Liberado| DB

    %% Relações Admin
    Admin -->|1. Informa e-mail/senha| F4
    F4 -->|Valida| Auth
    Auth -.->|Devolve Token 'Admin'| Admin
    
    Admin -->|2. Consulta Gráficos| F5
    Admin -->|3. Acessa Tabelas Restritas| F6
    F5 -- Requer Token 'Admin' --> Auth
    F6 -- Requer Token 'Admin' --> Auth
    Auth -->|Acesso Liberado| DB

    %% Estilos
    classDef ator fill:#f9f9f9,stroke:#333,stroke-width:2px;
    classDef auth fill:#ffe0b2,stroke:#f57c00,stroke-width:2px;
    classDef db fill:#c8e6c9,stroke:#388e3c,stroke-width:2px;
    
    class Coletor,Admin ator;
    class Auth auth;
    class DB db;
```

## Como Funciona a Restrição
1. O **Serviço de Autenticação** intercepta todas as chamadas.
2. Se o Token JWT tiver a *Claim* (permissão) de `Coletor`, ele só tem passe livre para o endpoint `POST /api/sync`.
3. Se o Token JWT tiver a *Claim* de `Admin`, ele tem passe livre para os endpoints `GET /api/familias`, `GET /api/alunos`, etc.
4. Se um *Coletor* tentar acessar as métricas do painel, a API barra na porta retornando `403 Forbidden` (Proibido).
