using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using ControleCaixa.Model;
using ControleCaixa.Model.Enums;
using ControleCaixa.Model.Interfaces;
using ControleCaixa.Model.DTOs;

namespace ControleCaixa.Data
{
    public class MovimentacaoRepository : IMovimentacaoRepository
    {
        public void Inserir(MovimentacaoCaixa movimentacao)
        {
            using (var connection = ConnectionHelper.CriarConexao())
            {
                var query = @"INSERT INTO MovimentacaoCaixa (Descricao, Tipo, Categoria, Valor, DataMovimento, Status) 
                              VALUES (@Descricao, @Tipo, @Categoria, @Valor, @DataMovimento, @Status)";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Descricao", movimentacao.Descricao);
                    command.Parameters.AddWithValue("@Tipo", (int)movimentacao.Tipo);
                    command.Parameters.AddWithValue("@Categoria", (object)movimentacao.Categoria ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Valor", movimentacao.Valor);
                    command.Parameters.AddWithValue("@DataMovimento", movimentacao.DataMovimento == default ? DateTime.Now : movimentacao.DataMovimento);
                    command.Parameters.AddWithValue("@Status", true); // Status 1 = Ativo

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public List<MovimentacaoCaixa> ListarAtivas()
        {
            var lista = new List<MovimentacaoCaixa>();

            using (var connection = ConnectionHelper.CriarConexao())
            {
                var query = "SELECT Id, Descricao, Tipo, Categoria, Valor, DataMovimento, Status FROM MovimentacaoCaixa WHERE Status = 1 ORDER BY DataMovimento DESC";

                using (var command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new MovimentacaoCaixa
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Descricao = reader["Descricao"].ToString(),
                                Tipo = (TipoMovimentacao)Convert.ToInt32(reader["Tipo"]),
                                Categoria = reader["Categoria"] != DBNull.Value ? reader["Categoria"].ToString() : null,
                                Valor = Convert.ToDecimal(reader["Valor"]),
                                DataMovimento = Convert.ToDateTime(reader["DataMovimento"]),
                                Status = Convert.ToBoolean(reader["Status"])
                            });
                        }
                    }
                }
            }

            return lista;
        }

        public List<MovimentacaoCaixa> ListarPorFiltros(string texto, DateTime? dataInicio, DateTime? dataFim)
        {
            var lista = new List<MovimentacaoCaixa>();

            using (var connection = ConnectionHelper.CriarConexao())
            {
                var query = "SELECT Id, Descricao, Tipo, Categoria, Valor, DataMovimento, Status FROM MovimentacaoCaixa WHERE Status = 1";

                if (!string.IsNullOrWhiteSpace(texto))
                {
                    query += " AND Descricao LIKE '%' + @Texto + '%'";
                }
                
                if (dataInicio.HasValue)
                {
                    query += " AND DataMovimento >= @DataInicio";
                }
                
                if (dataFim.HasValue)
                {
                    query += " AND DataMovimento <= @DataFim";
                }

                query += " ORDER BY DataMovimento DESC";

                using (var command = new SqlCommand(query, connection))
                {
                    if (!string.IsNullOrWhiteSpace(texto))
                        command.Parameters.AddWithValue("@Texto", texto);
                        
                    if (dataInicio.HasValue)
                        command.Parameters.AddWithValue("@DataInicio", dataInicio.Value.Date);
                        
                    if (dataFim.HasValue)
                        // Para pegar até as 23:59:59 do último dia selecionado
                        command.Parameters.AddWithValue("@DataFim", dataFim.Value.Date.AddDays(1).AddTicks(-1));

                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new MovimentacaoCaixa
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Descricao = reader["Descricao"].ToString(),
                                Tipo = (TipoMovimentacao)Convert.ToInt32(reader["Tipo"]),
                                Categoria = reader["Categoria"] != DBNull.Value ? reader["Categoria"].ToString() : null,
                                Valor = Convert.ToDecimal(reader["Valor"]),
                                DataMovimento = Convert.ToDateTime(reader["DataMovimento"]),
                                Status = Convert.ToBoolean(reader["Status"])
                            });
                        }
                    }
                }
            }

            return lista;
        }

        public MovimentacaoCaixa BuscarPorId(int id)
        {
            using (var connection = ConnectionHelper.CriarConexao())
            {
                var query = "SELECT Id, Descricao, Tipo, Categoria, Valor, DataMovimento, Status FROM MovimentacaoCaixa WHERE Id = @Id";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new MovimentacaoCaixa
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Descricao = reader["Descricao"].ToString(),
                                Tipo = (TipoMovimentacao)Convert.ToInt32(reader["Tipo"]),
                                Categoria = reader["Categoria"] != DBNull.Value ? reader["Categoria"].ToString() : null,
                                Valor = Convert.ToDecimal(reader["Valor"]),
                                DataMovimento = Convert.ToDateTime(reader["DataMovimento"]),
                                Status = Convert.ToBoolean(reader["Status"])
                            };
                        }
                    }
                }
            }

            return null;
        }

        public void Atualizar(MovimentacaoCaixa movimentacao)
        {
            using (var connection = ConnectionHelper.CriarConexao())
            {
                var query = @"UPDATE MovimentacaoCaixa 
                              SET Descricao = @Descricao, 
                                  Tipo = @Tipo, 
                                  Categoria = @Categoria, 
                                  Valor = @Valor, 
                                  DataMovimento = @DataMovimento 
                              WHERE Id = @Id";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", movimentacao.Id);
                    command.Parameters.AddWithValue("@Descricao", movimentacao.Descricao);
                    command.Parameters.AddWithValue("@Tipo", (int)movimentacao.Tipo);
                    command.Parameters.AddWithValue("@Categoria", (object)movimentacao.Categoria ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Valor", movimentacao.Valor);
                    command.Parameters.AddWithValue("@DataMovimento", movimentacao.DataMovimento);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public void Excluir(int id)
        {
            using (var connection = ConnectionHelper.CriarConexao())
            {
                var query = "UPDATE MovimentacaoCaixa SET Status = 0 WHERE Id = @Id";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public DashboardDTO ObterResumoDashboard()
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
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
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
    }
}
