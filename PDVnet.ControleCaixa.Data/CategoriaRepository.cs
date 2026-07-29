using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using ControleCaixa.Model;
using ControleCaixa.Model.Interfaces;

namespace ControleCaixa.Data
{
    public class CategoriaRepository : ICategoriaRepository
    {
        public async Task<List<Categoria>> ListarTodasAsync()
        {
            var lista = new List<Categoria>();
            using (var connection = ConnectionHelper.CriarConexao())
            {
                var cmd = new SqlCommand("SELECT Id, Nome FROM Categoria ORDER BY Nome", connection);
                await connection.OpenAsync();
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        lista.Add(new Categoria
                        {
                            Id = reader.GetInt32(0),
                            Nome = reader.GetString(1)
                        });
                    }
                }
            }
            return lista;
        }

        public async Task AdicionarAsync(Categoria categoria)
        {
            using (var connection = ConnectionHelper.CriarConexao())
            {
                var cmd = new SqlCommand("INSERT INTO Categoria (Nome) VALUES (@Nome)", connection);
                cmd.Parameters.AddWithValue("@Nome", categoria.Nome);
                await connection.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task DeletarAsync(int id)
        {
            using (var connection = ConnectionHelper.CriarConexao())
            {
                var cmd = new SqlCommand("DELETE FROM Categoria WHERE Id = @Id", connection);
                cmd.Parameters.AddWithValue("@Id", id);
                await connection.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task<bool> EmUsoAsync(int id)
        {
            using (var connection = ConnectionHelper.CriarConexao())
            {
                var cmd = new SqlCommand("SELECT COUNT(1) FROM MovimentacaoCaixa WHERE CategoriaId = @Id", connection);
                cmd.Parameters.AddWithValue("@Id", id);
                await connection.OpenAsync();
                var result = await cmd.ExecuteScalarAsync();
                return (int)result > 0;
            }
        }
    }
}
