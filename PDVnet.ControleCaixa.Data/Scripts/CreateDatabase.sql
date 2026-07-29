IF DB_ID('PDVnetControleCaixa') IS NULL
    CREATE DATABASE PDVnetControleCaixa;
GO

USE PDVnetControleCaixa;
GO

IF OBJECT_ID('dbo.Categoria', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Categoria (
        Id   INT          IDENTITY(1,1) PRIMARY KEY,
        Nome VARCHAR(100) NOT NULL UNIQUE
    );

    INSERT INTO dbo.Categoria (Nome) VALUES 
        ('Vendas'), ('Pagamentos'), ('Serviços Agregados'), 
        ('Recebimento de Fornecedores'), ('Contas de Consumo'), 
        ('Salário'), ('Manutenção e Reparos'), ('Outros');
END
GO

IF OBJECT_ID('dbo.MovimentacaoCaixa', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.MovimentacaoCaixa (
        Id            INT           IDENTITY(1,1) PRIMARY KEY,
        Descricao     VARCHAR(200)                NOT NULL,
        Tipo          INT                         NOT NULL CHECK (Tipo IN (1, 2)),
        CategoriaId   INT                         NULL FOREIGN KEY REFERENCES dbo.Categoria(Id),
        Valor         DECIMAL(10,2)               NOT NULL CHECK (Valor > 0),
        DataMovimento DATETIME                    NOT NULL DEFAULT GETDATE(),
        Status        BIT                         NOT NULL DEFAULT 1               
    );
END
GO