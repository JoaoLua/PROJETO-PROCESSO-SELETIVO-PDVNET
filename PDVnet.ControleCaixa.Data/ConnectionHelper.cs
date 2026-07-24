using System;
using System.Collections.Generic;
using System.Configuration;
using Microsoft.Data.SqlClient;
using System.Text;

namespace ControleCaixa.Data
{
    public static class ConnectionHelper
    {
        private const string _ConnectionStringName = "PDVnetControleCaixa";
        private static readonly string _connectionString = ObterConnectionString();

        public static SqlConnection CriarConexao()
        {
            return new SqlConnection(_connectionString);
        }
        private static string ObterConnectionString()
        {
            var settings = ConfigurationManager.ConnectionStrings[_ConnectionStringName];

            if (settings == null || string.IsNullOrWhiteSpace(settings.ConnectionString))
            {
                throw new InvalidOperationException(
                    $"Connection string '{_ConnectionStringName}' não encontrada no App.config.");
            }

            return settings.ConnectionString;
        }
    }
}