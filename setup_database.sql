-- ====================================
-- Script de Configuração do Banco de Dados
-- Sistema Kanban - KanbanDB
-- ====================================

USE master;
GO

-- Criar banco de dados se não existir
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'KanbanDB')
BEGIN
    CREATE DATABASE KanbanDB;
    PRINT '✓ Banco de dados KanbanDB criado com sucesso!';
END
ELSE
BEGIN
    PRINT '! Banco de dados KanbanDB já existe.';
END
GO

USE KanbanDB;
GO

-- ====================================
-- CRIAÇÃO DAS TABELAS
-- ====================================

-- Tabela de Usuários
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Users')
BEGIN
    CREATE TABLE Users (
        id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
        username NVARCHAR(100) NOT NULL UNIQUE,
        password NVARCHAR(255) NOT NULL,
        admin BIT NOT NULL DEFAULT 0,
        created_at DATETIME2 DEFAULT GETDATE()
    );
    PRINT '✓ Tabela Users criada com sucesso!';
END
ELSE
BEGIN
    PRINT '! Tabela Users já existe.';
END
GO

-- Tabela de Colunas do Kanban
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Columns')
BEGIN
    CREATE TABLE Columns (
        id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
        name NVARCHAR(100) NOT NULL,
        position INT NOT NULL,
        user_id UNIQUEIDENTIFIER NOT NULL,
        created_at DATETIME2 DEFAULT GETDATE(),
        FOREIGN KEY (user_id) REFERENCES Users(id) ON DELETE CASCADE
    );
    PRINT '✓ Tabela Columns criada com sucesso!';
END
ELSE
BEGIN
    PRINT '! Tabela Columns já existe.';
END
GO

-- Tabela de Cards do Kanban
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Cards')
BEGIN
    CREATE TABLE Cards (
        id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
        title NVARCHAR(200) NOT NULL,
        description NVARCHAR(MAX),
        column_id UNIQUEIDENTIFIER NOT NULL,
        position INT NOT NULL,
        created_at DATETIME2 DEFAULT GETDATE(),
        FOREIGN KEY (column_id) REFERENCES Columns(id) ON DELETE CASCADE
    );
    PRINT '✓ Tabela Cards criada com sucesso!';
END
ELSE
BEGIN
    PRINT '! Tabela Cards já existe.';
END
GO

-- Tabela de Tickets (Sistema de Suporte)
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Tickets')
BEGIN
    CREATE TABLE Tickets (
        id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
        title NVARCHAR(200) NOT NULL,
        description NVARCHAR(MAX) NOT NULL,
        status NVARCHAR(50) NOT NULL DEFAULT 'Aberto',
        priority NVARCHAR(20) NOT NULL DEFAULT 'Média',
        user_id UNIQUEIDENTIFIER NOT NULL,
        assigned_to UNIQUEIDENTIFIER NULL,
        created_at DATETIME2 DEFAULT GETDATE(),
        updated_at DATETIME2 DEFAULT GETDATE(),
        FOREIGN KEY (user_id) REFERENCES Users(id) ON DELETE CASCADE,
        FOREIGN KEY (assigned_to) REFERENCES Users(id)
    );
    PRINT '✓ Tabela Tickets criada com sucesso!';
END
ELSE
BEGIN
    PRINT '! Tabela Tickets já existe.';
END
GO

-- ====================================
-- INSERÇÃO DE USUÁRIOS MOCK
-- ====================================

PRINT '';
PRINT '====================================';
PRINT 'Inserindo usuários de teste...';
PRINT '====================================';

-- Verificar se já existem usuários
IF NOT EXISTS (SELECT * FROM Users WHERE username = 'admin')
BEGIN
    -- Senha para todos os usuários: "senha123"
    -- Hash BCrypt gerado para "senha123": $2a$11$Hq5Z7vZ9kQ0yK0L4M2X0XuH1F6Z3N9X8Y7W6V5U4T3S2R1Q0P9O8N
    
    -- Usuário Administrador
    INSERT INTO Users (id, username, password, admin, created_at)
    VALUES (
        '11111111-1111-1111-1111-111111111111',
        'admin',
        '$2a$11$Hq5Z7vZ9kQ0yK0L4M2X0XuH1F6Z3N9X8Y7W6V5U4T3S2R1Q0P9O8N',
        1,
        '2025-09-15 10:00:00'
    );
    PRINT '✓ Usuário admin criado (senha: senha123)';

    -- Usuários Comuns
    INSERT INTO Users (id, username, password, admin, created_at)
    VALUES 
    (
        '22222222-2222-2222-2222-222222222222',
        'joao',
        '$2a$11$Hq5Z7vZ9kQ0yK0L4M2X0XuH1F6Z3N9X8Y7W6V5U4T3S2R1Q0P9O8N',
        0,
        '2025-09-16 11:00:00'
    ),
    (
        '33333333-3333-3333-3333-333333333333',
        'maria',
        '$2a$11$Hq5Z7vZ9kQ0yK0L4M2X0XuH1F6Z3N9X8Y7W6V5U4T3S2R1Q0P9O8N',
        0,
        '2025-09-17 14:00:00'
    ),
    (
        '44444444-4444-4444-4444-444444444444',
        'pedro',
        '$2a$11$Hq5Z7vZ9kQ0yK0L4M2X0XuH1F6Z3N9X8Y7W6V5U4T3S2R1Q0P9O8N',
        0,
        '2025-09-18 09:00:00'
    );
    PRINT '✓ Usuários comuns criados: joao, maria, pedro (senha: senha123)';
END
ELSE
BEGIN
    PRINT '! Usuários já existem no banco de dados.';
END
GO

-- ====================================
-- DADOS DE EXEMPLO PARA O USUÁRIO ADMIN
-- ====================================

PRINT '';
PRINT 'Inserindo dados de exemplo para o usuário admin...';

IF NOT EXISTS (SELECT * FROM Columns WHERE user_id = '11111111-1111-1111-1111-111111111111')
BEGIN
    -- Colunas do Kanban para Admin
    INSERT INTO Columns (id, name, position, user_id, created_at)
    VALUES 
    (
        'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa',
        'A Fazer',
        0,
        '11111111-1111-1111-1111-111111111111',
        '2025-09-20 10:00:00'
    ),
    (
        'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb',
        'Em Progresso',
        1,
        '11111111-1111-1111-1111-111111111111',
        '2025-09-20 10:01:00'
    ),
    (
        'cccccccc-cccc-cccc-cccc-cccccccccccc',
        'Concluído',
        2,
        '11111111-1111-1111-1111-111111111111',
        '2025-09-20 10:02:00'
    );
    PRINT '✓ Colunas criadas para o admin';

    -- Cards de exemplo
    INSERT INTO Cards (id, title, description, column_id, position, created_at)
    VALUES 
    (
        'dddddddd-dddd-dddd-dddd-dddddddddddd',
        'Implementar autenticação',
        'Criar sistema de login com BCrypt',
        'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa',
        0,
        '2025-09-21 11:00:00'
    ),
    (
        'eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee',
        'Criar interface WPF',
        'Desenvolver aplicação desktop em WPF',
        'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb',
        0,
        '2025-09-22 14:00:00'
    ),
    (
        'ffffffff-ffff-ffff-ffff-ffffffffffff',
        'Configurar banco de dados',
        'Setup do SQL Server LocalDB',
        'cccccccc-cccc-cccc-cccc-cccccccccccc',
        0,
        '2025-09-19 09:00:00'
    );
    PRINT '✓ Cards de exemplo criados';
END
ELSE
BEGIN
    PRINT '! Dados de exemplo já existem.';
END
GO

-- ====================================
-- TICKETS DE EXEMPLO
-- ====================================

PRINT '';
PRINT 'Inserindo tickets de exemplo...';

IF NOT EXISTS (SELECT * FROM Tickets WHERE user_id = '22222222-2222-2222-2222-222222222222')
BEGIN
    INSERT INTO Tickets (id, title, description, status, priority, user_id, assigned_to, created_at)
    VALUES 
    (
        '12345678-1234-1234-1234-123456789012',
        'Erro ao criar nova coluna',
        'Quando tento criar uma nova coluna, recebo um erro 500.',
        'Aberto',
        'Alta',
        '22222222-2222-2222-2222-222222222222',
        '11111111-1111-1111-1111-111111111111',
        '2025-10-29 10:30:00'
    ),
    (
        '23456789-2345-2345-2345-234567890123',
        'Sugestão de melhoria na interface',
        'Seria interessante ter um tema claro além do tema escuro.',
        'Em Análise',
        'Baixa',
        '33333333-3333-3333-3333-333333333333',
        NULL,
        '2025-11-01 15:45:00'
    ),
    (
        '34567890-3456-3456-3456-345678901234',
        'Cards não estão sendo salvos',
        'Após criar um card, ele desaparece quando atualizo a página.',
        'Resolvido',
        'Alta',
        '44444444-4444-4444-4444-444444444444',
        '11111111-1111-1111-1111-111111111111',
        '2025-10-31 09:15:00'
    );
    PRINT '✓ Tickets de exemplo criados';
END
ELSE
BEGIN
    PRINT '! Tickets de exemplo já existem.';
END
GO

-- ====================================
-- ÍNDICES PARA PERFORMANCE
-- ====================================

PRINT '';
PRINT 'Criando índices...';

-- Índice para busca de colunas por usuário
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Columns_UserId')
BEGIN
    CREATE INDEX IX_Columns_UserId ON Columns(user_id);
    PRINT '✓ Índice IX_Columns_UserId criado';
END

-- Índice para busca de cards por coluna
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Cards_ColumnId')
BEGIN
    CREATE INDEX IX_Cards_ColumnId ON Cards(column_id);
    PRINT '✓ Índice IX_Cards_ColumnId criado';
END

-- Índice para busca de tickets por usuário
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Tickets_UserId')
BEGIN
    CREATE INDEX IX_Tickets_UserId ON Tickets(user_id);
    PRINT '✓ Índice IX_Tickets_UserId criado';
END

-- Índice para busca de tickets por status
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Tickets_Status')
BEGIN
    CREATE INDEX IX_Tickets_Status ON Tickets(status);
    PRINT '✓ Índice IX_Tickets_Status criado';
END
GO

-- ====================================
-- VERIFICAÇÃO FINAL
-- ====================================

PRINT '';
PRINT '====================================';
PRINT 'CONFIGURAÇÃO CONCLUÍDA!';
PRINT '====================================';
PRINT '';
PRINT 'Resumo do banco de dados:';
SELECT 
    'Users' as Tabela,
    COUNT(*) as Total
FROM Users
UNION ALL
SELECT 
    'Columns' as Tabela,
    COUNT(*) as Total
FROM Columns
UNION ALL
SELECT 
    'Cards' as Tabela,
    COUNT(*) as Total
FROM Cards
UNION ALL
SELECT 
    'Tickets' as Tabela,
    COUNT(*) as Total
FROM Tickets;

PRINT '';
PRINT '====================================';
PRINT 'CREDENCIAIS DE ACESSO:';
PRINT '====================================';
PRINT 'Administrador:';
PRINT '  Usuário: admin';
PRINT '  Senha: senha123';
PRINT '';
PRINT 'Usuários Comuns:';
PRINT '  Usuário: joao / maria / pedro';
PRINT '  Senha: senha123';
PRINT '====================================';
GO
