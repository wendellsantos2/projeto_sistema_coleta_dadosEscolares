-- Habilitar a extensão para geração de UUIDs (necessário no PostgreSQL)
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

-- ==========================================
-- 1. Tabela: Familias
-- ==========================================
CREATE TABLE familias (
    id_familia UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    endereco VARCHAR(255),
    bairro VARCHAR(100),
    comunidade VARCHAR(100),
    qtd_moradores INT,
    renda_familiar_mensal DECIMAL(10, 2),
    recebe_beneficio_social BOOLEAN,
    beneficio_social VARCHAR(255),
    possui_internet_casa BOOLEAN,
    tipo_acesso_internet VARCHAR(100),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- ==========================================
-- 2. Tabela: Alunos
-- ==========================================
CREATE TABLE alunos (
    id_aluno UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    id_familia UUID NOT NULL,
    nome_aluno VARCHAR(255) NOT NULL,
    data_nascimento DATE,
    sexo VARCHAR(50),
    cpf_aluno VARCHAR(20) UNIQUE,
    necessidade_educacional_especial BOOLEAN DEFAULT FALSE,
    descricao_necessidade TEXT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_aluno_familia FOREIGN KEY (id_familia) REFERENCES familias(id_familia) ON DELETE CASCADE
);

-- ==========================================
-- 3. Tabela: Responsaveis
-- ==========================================
CREATE TABLE responsaveis (
    id_responsavel UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    nome_responsavel VARCHAR(255) NOT NULL,
    cpf_responsavel VARCHAR(20) UNIQUE,
    telefone_responsavel VARCHAR(20),
    email_responsavel VARCHAR(255),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- ==========================================
-- 4. Tabela: Aluno_Responsavel (Relação N:M)
-- ==========================================
CREATE TABLE aluno_responsavel (
    id_aluno UUID NOT NULL,
    id_responsavel UUID NOT NULL,
    parentesco_responsavel VARCHAR(100),
    PRIMARY KEY (id_aluno, id_responsavel),
    CONSTRAINT fk_ar_aluno FOREIGN KEY (id_aluno) REFERENCES alunos(id_aluno) ON DELETE CASCADE,
    CONSTRAINT fk_ar_responsavel FOREIGN KEY (id_responsavel) REFERENCES responsaveis(id_responsavel) ON DELETE CASCADE
);

-- ==========================================
-- 5. Tabela: Matriculas (Dados Escolares Temporais)
-- ==========================================
CREATE TABLE matriculas (
    id_matricula UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    id_aluno UUID NOT NULL,
    ano_letivo INT NOT NULL,
    ano_serie VARCHAR(50),
    turno VARCHAR(50),
    frequencia_escolar_pct DECIMAL(5, 2),
    meio_transporte_escola VARCHAR(100),
    tempo_deslocamento_min INT,
    CONSTRAINT fk_matricula_aluno FOREIGN KEY (id_aluno) REFERENCES alunos(id_aluno) ON DELETE CASCADE
);

-- ==========================================
-- 6. Tabela: Registros_Coleta (Auditoria e Sync Mobile)
-- ==========================================
CREATE TABLE registros_coleta (
    id_registro UUID PRIMARY KEY, -- ID enviado pelo App Mobile
    id_aluno UUID NOT NULL,
    data_coleta TIMESTAMP NOT NULL,
    observacao TEXT,
    status_sincronizacao VARCHAR(50) DEFAULT 'SINCRONIZADO',
    CONSTRAINT fk_registro_aluno FOREIGN KEY (id_aluno) REFERENCES alunos(id_aluno) ON DELETE CASCADE
);

-- ==========================================
-- Índices de Performance
-- ==========================================
CREATE INDEX idx_alunos_familia ON alunos(id_familia);
CREATE INDEX idx_matriculas_aluno ON matriculas(id_aluno);
CREATE INDEX idx_registros_aluno ON registros_coleta(id_aluno);
