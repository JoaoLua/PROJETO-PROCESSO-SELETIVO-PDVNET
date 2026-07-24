IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'PDVnetControleCaixa')
BEGIN
    CREATE DATABASE PDVnetControleCaixa;
END
GO

USE PDVnetControleCaixa;
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.objects
    WHERE object_id = OBJECT_ID(N'dbo.MovimentacaoCaixa') AND type = N'U'
)
BEGIN
    CREATE TABLE dbo.MovimentacaoCaixa
    (
        Id            INT           IDENTITY(1,1) NOT NULL,
        Descricao     VARCHAR(200)                NOT NULL,
        Tipo          INT                         NOT NULL,   -- 1 = Entrada | 2 = Saída
        Categoria     VARCHAR(100)                NULL,
        Valor         DECIMAL(10,2)               NOT NULL,
        DataMovimento DATETIME                    NOT NULL
            CONSTRAINT DF_MovimentacaoCaixa_DataMovimento DEFAULT (GETDATE()),
        Status        BIT                         NOT NULL
            CONSTRAINT DF_MovimentacaoCaixa_Status DEFAULT (1),  -- 1 = Ativo | 0 = Inativo

        CONSTRAINT PK_MovimentacaoCaixa PRIMARY KEY CLUSTERED (Id),
        CONSTRAINT CK_MovimentacaoCaixa_Valor CHECK (Valor > 0),
        CONSTRAINT CK_MovimentacaoCaixa_Tipo  CHECK (Tipo IN (1, 2))
    );
END
GO