using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using ControleCaixa.Model;
using ControleCaixa.Model.Enums;
using ControleCaixa.Model.Interfaces;
using ControleCaixa.Model.DTOs;

namespace ControleCaixa.Data
{
    public class MovimentacaoRepository : IMovimentacaoRepository
    {
        public async Task InserirAsync(MovimentacaoCaixa movimentacao)
        {
            using (var connection = ConnectionHelper.CriarConexao())
            {
                var query = @"INSERT INTO MovimentacaoCaixa (Descricao, Tipo, CategoriaId, Valor, DataMovimento, Status) 
                              VALUES (@Descricao, @Tipo, @CategoriaId, @Valor, @DataMovimento, @Status)";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Descricao", movimentacao.Descricao);
                    command.Parameters.AddWithValue("@Tipo", (int)movimentacao.Tipo);
                    command.Parameters.AddWithValue("@CategoriaId", (object)movimentacao.CategoriaId ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Valor", movimentacao.Valor);
                    command.Parameters.AddWithValue("@DataMovimento", movimentacao.DataMovimento == default ? DateTime.Now : movimentacao.DataMovimento);
                    command.Parameters.AddWithValue("@Status", true); 

                    await connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<List<MovimentacaoCaixa>> ListarAtivasAsync()
        {
            var lista = new List<MovimentacaoCaixa>();

            using (var connection = ConnectionHelper.CriarConexao())
            {
                var query = @"SELECT M.Id, M.Descricao, M.Tipo, M.CategoriaId, C.Nome AS CategoriaNome, M.Valor, M.DataMovimento, M.Status 
                              FROM MovimentacaoCaixa M 
                              LEFT JOIN Categoria C ON M.CategoriaId = C.Id 
                              WHERE M.Status = 1 
                              ORDER BY M.DataMovimento DESC";

                using (var command = new SqlCommand(query, connection))
                {
                    await connection.OpenAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            lista.Add(MapearMovimentacao(reader));
                        }
                    }
                }
            }

            return lista;
        }

        public async Task<List<MovimentacaoCaixa>> ListarPorFiltrosAsync(string texto, DateTime? dataInicio, DateTime? dataFim, string categoria = null, TipoMovimentacao? tipo = null, bool? ativo = true)
        {
            var lista = new List<MovimentacaoCaixa>();

            using (var connection = ConnectionHelper.CriarConexao())
            {
                var query = @"SELECT M.Id, M.Descricao, M.Tipo, M.CategoriaId, C.Nome AS CategoriaNome, M.Valor, M.DataMovimento, M.Status 
                              FROM MovimentacaoCaixa M 
                              LEFT JOIN Categoria C ON M.CategoriaId = C.Id 
                              WHERE 1=1";

                if (ativo.HasValue)
                {
                    query += " AND M.Status = @Status";
                }

                if (!string.IsNullOrWhiteSpace(texto))
                {
                    query += " AND M.Descricao LIKE '%' + @Texto + '%'";
                }
                
                if (dataInicio.HasValue)
                {
                    query += " AND M.DataMovimento >= @DataInicio";
                }
                
                if (dataFim.HasValue)
                {
                    query += " AND M.DataMovimento <= @DataFim";
                }

                if (!string.IsNullOrWhiteSpace(categoria))
                {
                    query += " AND C.Nome = @Categoria";
                }

                if (tipo.HasValue)
                {
                    query += " AND M.Tipo = @Tipo";
                }

                query += " ORDER BY M.DataMovimento DESC";

                using (var command = new SqlCommand(query, connection))
                {
                    if (ativo.HasValue)
                        command.Parameters.AddWithValue("@Status", ativo.Value);

                    if (!string.IsNullOrWhiteSpace(texto))
                        command.Parameters.AddWithValue("@Texto", texto);
                        
                    if (dataInicio.HasValue)
                        command.Parameters.AddWithValue("@DataInicio", dataInicio.Value.Date);
                        
                    if (dataFim.HasValue)
                        command.Parameters.AddWithValue("@DataFim", dataFim.Value.Date.AddDays(1).AddTicks(-1));

                    if (!string.IsNullOrWhiteSpace(categoria))
                        command.Parameters.AddWithValue("@Categoria", categoria);

                    if (tipo.HasValue)
                        command.Parameters.AddWithValue("@Tipo", (int)tipo.Value);

                    await connection.OpenAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            lista.Add(MapearMovimentacao(reader));
                        }
                    }
                }
            }

            return lista;
        }

        public async Task<MovimentacaoCaixa> BuscarPorIdAsync(int id)
        {
            using (var connection = ConnectionHelper.CriarConexao())
            {
                var query = @"SELECT M.Id, M.Descricao, M.Tipo, M.CategoriaId, C.Nome AS CategoriaNome, M.Valor, M.DataMovimento, M.Status 
                              FROM MovimentacaoCaixa M 
                              LEFT JOIN Categoria C ON M.CategoriaId = C.Id 
                              WHERE M.Id = @Id";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    await connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return MapearMovimentacao(reader);
                        }
                    }
                }
            }

            return null;
        }

        public async Task AtualizarAsync(MovimentacaoCaixa movimentacao)
        {
            using (var connection = ConnectionHelper.CriarConexao())
            {
                var query = @"UPDATE MovimentacaoCaixa 
                              SET Descricao = @Descricao, 
                                  Tipo = @Tipo, 
                                  CategoriaId = @CategoriaId, 
                                  Valor = @Valor, 
                                  DataMovimento = @DataMovimento 
                              WHERE Id = @Id";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", movimentacao.Id);
                    command.Parameters.AddWithValue("@Descricao", movimentacao.Descricao);
                    command.Parameters.AddWithValue("@Tipo", (int)movimentacao.Tipo);
                    command.Parameters.AddWithValue("@CategoriaId", (object)movimentacao.CategoriaId ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Valor", movimentacao.Valor);
                    command.Parameters.AddWithValue("@DataMovimento", movimentacao.DataMovimento);

                    await connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task ExcluirAsync(int id)
        {
            using (var connection = ConnectionHelper.CriarConexao())
            {
                var query = "UPDATE MovimentacaoCaixa SET Status = 0 WHERE Id = @Id";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    
                    await connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task ReativarAsync(int id)
        {
            using (var connection = ConnectionHelper.CriarConexao())
            {
                var query = "UPDATE MovimentacaoCaixa SET Status = 1 WHERE Id = @Id";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    
                    await connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<DashboardDTO> ObterResumoDashboardAsync()
        {
            var dto = new DashboardDTO();

            using (var connection = ConnectionHelper.CriarConexao())
            {
                var query = @"SELECT 
                                COUNT(*) AS TotalMovimentacoes,
                                ISNULL(SUM(CASE WHEN Tipo = 1 THEN Valor ELSE 0 END), 0) AS TotalEntradas,
                                ISNULL(SUM(CASE WHEN Tipo = 2 THEN Valor ELSE 0 END), 0) AS TotalSaidas,
                                ISNULL(SUM(CASE WHEN Tipo = 1 THEN Valor ELSE 0 END), 0) - 
                                ISNULL(SUM(CASE WHEN Tipo = 2 THEN Valor ELSE 0 END), 0) AS SaldoTotal
                              FROM MovimentacaoCaixa 
                              WHERE Status = 1";

                using (var command = new SqlCommand(query, connection))
                {
                    await connection.OpenAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            dto.TotalMovimentacoes = Convert.ToInt32(reader["TotalMovimentacoes"]);
                            dto.TotalEntradas = Convert.ToDecimal(reader["TotalEntradas"]);
                            dto.TotalSaidas = Convert.ToDecimal(reader["TotalSaidas"]);
                            dto.SaldoTotal = Convert.ToDecimal(reader["SaldoTotal"]);
                        }
                    }
                }
            }

            return dto;
        }

        private MovimentacaoCaixa MapearMovimentacao(SqlDataReader reader)
        {
            return new MovimentacaoCaixa
            {
                Id = Convert.ToInt32(reader["Id"]),
                Descricao = reader["Descricao"].ToString(),
                Tipo = (TipoMovimentacao)Convert.ToInt32(reader["Tipo"]),
                CategoriaId = reader["CategoriaId"] != DBNull.Value ? Convert.ToInt32(reader["CategoriaId"]) : (int?)null,
                Categoria = reader["CategoriaNome"] != DBNull.Value ? reader["CategoriaNome"].ToString() : null,
                Valor = Convert.ToDecimal(reader["Valor"]),
                DataMovimento = Convert.ToDateTime(reader["DataMovimento"]),
                Status = Convert.ToBoolean(reader["Status"])
            };
        }
    }
}
