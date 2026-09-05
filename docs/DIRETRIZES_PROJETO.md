# Diretrizes e Modelagem do Projeto (Coleta Escolar)

Este documento serve como referência base de regras de negócio, modelagem e casos de uso para o desenvolvimento do Backend, Frontend e Mobile. Sempre consulte este arquivo antes de criar novos fluxos no projeto.

## 1. Regras de Negócio Gerais
*   **Offline-First:** O aplicativo móvel DEVE funcionar sem internet. Todos os registros coletados serão salvos em um banco local (`SQLite`/`Isar`) com status pendente.
*   **ID Descentralizado:** Para viabilizar o offline-first, NUNCA confie no autoincremento do banco central (PostgreSQL). Toda nova `Familia`, `Aluno` ou `Registro` deve receber um `UUIDv4` gerado pelo próprio celular na hora da coleta.
*   **Sincronização:** Quando a internet voltar, o celular enviará os registros pendentes para o Endpoint `/api/sync`. Se o envio for bem-sucedido (Status HTTP 2xx), o celular marca o registro local como `sincronizado`.

## 2. Entidades Principais (Domain)

Os dados capturados do formulário plano devem ser normalizados no Backend para as seguintes entidades:

*   **Família (`id_familia` - UUID):**
    *   `endereco`, `bairro`, `comunidade`, `qtd_moradores`, `renda_familiar_mensal`, `recebe_beneficio_social`, `beneficio_social`, `possui_internet_casa`, `tipo_acesso_internet`.
*   **Aluno (`id_aluno` - UUID):**
    *   `id_familia` (Foreign Key), `nome_aluno`, `data_nascimento`, `sexo`, `cpf_aluno`, `necessidade_educacional_especial`, `descricao_necessidade`.
*   **Responsável (`id_responsavel` - UUID ou CPF):**
    *   `nome_responsavel`, `cpf_responsavel`, `telefone_responsavel`, `email_responsavel`.
*   **Aluno_Responsavel (N:M):**
    *   Relaciona o aluno com o responsável, contendo o `parentesco_responsavel`.
*   **Matrícula (`id_matricula` - UUID):**
    *   `id_aluno` (FK), `ano_serie`, `turno`, `frequencia_escolar_pct`, `meio_transporte_escola`, `tempo_deslocamento_min`.
*   **Registro Coleta (`id_registro` - UUID):**
    *   Tabela de auditoria: Quando a coleta foi feita (`data_coleta`) e `observacao`.

## 3. Payload de Sincronização (O Padrão Ouro)
O formato JSON abaixo é o contrato que o Flutter mandará para a API, e a API deverá ser capaz de processar e inserir nas tabelas relacionais de uma só vez.

```json
{
  "id_registro": "uuid",
  "data_coleta": "2026-09-05T14:00:00Z",
  "observacao": "Sem observações adicionais",
  "familia": {
    "id_familia": "uuid",
    "endereco": "Rua Rio Negro, 101",
    "bairro": "Alvorada",
    ...
  },
  "aluno": {
    "id_aluno": "uuid",
    "nome_aluno": "Ana Souza Santos",
    "cpf_aluno": "700.000.010-01",
    ...
  },
  "responsavel": {
    "cpf_responsavel": "400.000.000-01",
    "nome_responsavel": "Alice Souza Silva",
    ...
  },
  "matricula": {
    "ano_serie": "3º ano EF",
    "turno": "Vespertino",
    ...
  }
}
```

## 4. O Que Fazer ao Desenvolver
1. **Backend:** Ao codificar novos endpoints, leia a estrutura JSON acima. Desenvolva serviços que consigam fazer insert em múltiplas tabelas numa mesma transação (`BeginTransaction`).
2. **Mobile (Flutter):** Garanta as validações básicas no formulário e gere os UUIDs na hora de salvar.
3. **Web (Angular):** Use as rotas GET para popular tabelas e usar bibliotecas gráficas. Evite lógica complexa de processamento no frontend.
