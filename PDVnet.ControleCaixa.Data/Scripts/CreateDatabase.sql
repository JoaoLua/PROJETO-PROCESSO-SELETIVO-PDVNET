IF DB_ID('PDVnetControleCaixa') IS NULL
    CREATE DATABASE PDVnetControleCaixa;
GO

USE PDVnetControleCaixa;
GO

IF OBJECT_ID('dbo.MovimentacaoCaixa', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.MovimentacaoCaixa (
        Id            INT           IDENTITY(1,1) PRIMARY KEY,
        Descricao     VARCHAR(200)                NOT NULL,
        Tipo          INT                         NOT NULL CHECK (Tipo IN (1, 2)),
        Categoria     VARCHAR(100)                NULL,
        Valor         DECIMAL(10,2)               NOT NULL CHECK (Valor > 0),
        DataMovimento DATETIME                    NOT NULL DEFAULT GETDATE(),
        Status        BIT                         NOT NULL DEFAULT 1               
    );
END
GO