USE PDVnetControleCaixa;
GO

INSERT INTO MovimentacaoCaixa (Descricao, Tipo, Categoria, Valor, DataMovimento, Status)
VALUES
    ('Venda de produto - Cliente Mercado Local',      1, 'Vendas',         1250.00, '2026-05-02 09:15:00', 1),
    ('Pagamento fornecedor - Distribuidora ABC',       2, 'Fornecedores',   3200.50, '2026-05-03 14:30:00', 1),
    ('Aluguel do galpão',                              2, 'Despesas Fixas', 4500.00, '2026-05-05 08:00:00', 1),
    ('Venda balcão - dia 06/05',                       1, 'Vendas',          890.30, '2026-05-06 17:45:00', 1),
    ('Conta de energia elétrica',                      2, 'Despesas Fixas',  780.90, '2026-05-07 10:00:00', 1),
    ('Venda via e-commerce',                           1, 'Vendas',         2100.00, '2026-05-10 11:20:00', 1),
    ('Pagamento fornecedor - Embalagens Silva',        2, 'Fornecedores',   1560.75, '2026-05-12 09:40:00', 1),
    ('Salário - equipe administrativa',                2, 'Salários',       8500.00, '2026-05-15 12:00:00', 1),
    ('Venda no atacado',                               1, 'Vendas',         5400.00, '2026-05-18 15:10:00', 1),
    ('Manutenção de equipamentos',                     2, 'Manutenção',      620.00, '2026-05-20 13:25:00', 1),
    ('Venda balcão - dia 22/05',                       1, 'Vendas',          430.00, '2026-05-22 16:00:00', 1),
    ('Pagamento de imposto - ICMS',                    2, 'Impostos',       2340.00, '2026-05-25 10:30:00', 1),
    ('Venda para revenda',                             1, 'Vendas',         3100.00, '2026-05-28 09:00:00', 1),
    ('Compra de material de limpeza',                  2, 'Fornecedores',    210.40, '2026-05-29 11:00:00', 0),
    ('Venda de produto - Cliente Mercado Central',     1, 'Vendas',         1780.00, '2026-06-01 09:15:00', 1),
    ('Pagamento fornecedor - Distribuidora XPTO',      2, 'Fornecedores',   2890.00, '2026-06-03 14:00:00', 1),
    ('Aluguel do galpão',                              2, 'Despesas Fixas', 4500.00, '2026-06-05 08:00:00', 1),
    ('Venda via e-commerce',                           1, 'Vendas',         1990.50, '2026-06-08 13:40:00', 1),
    ('Conta de água',                                  2, 'Despesas Fixas',  340.20, '2026-06-09 10:00:00', 1),
    ('Venda no atacado',                               1, 'Vendas',         6200.00, '2026-06-12 15:30:00', 1),
    ('Salário - equipe administrativa',                2, 'Salários',       8500.00, '2026-06-15 12:00:00', 1),
    ('Marketing - anúncios redes sociais',             2, 'Marketing',       950.00, '2026-06-17 09:00:00', 1),
    ('Venda balcão - dia 19/06',                       1, 'Vendas',          670.00, '2026-06-19 16:20:00', 1),
    ('Pagamento fornecedor - Embalagens Silva',        2, 'Fornecedores',   1420.00, '2026-06-22 09:40:00', 1),
    ('Venda para revenda',                             1, 'Vendas',         2950.00, '2026-06-25 10:10:00', 1),
    ('Manutenção de equipamentos',                     2, 'Manutenção',      480.00, '2026-06-27 13:00:00', 1),
    ('Devolução de cliente (registro incorreto)',      1, 'Vendas',          150.00, '2026-06-28 09:00:00', 0),
    ('Venda de produto - Cliente Mercado Local',       1, 'Vendas',         2340.00, '2026-07-02 09:15:00', 1),
    ('Pagamento de imposto - ISS',                     2, 'Impostos',        980.00, '2026-07-05 11:00:00', 1),
    ('Venda via e-commerce',                           1, 'Vendas',         3300.00, '2026-07-10 14:20:00', 1);
GO