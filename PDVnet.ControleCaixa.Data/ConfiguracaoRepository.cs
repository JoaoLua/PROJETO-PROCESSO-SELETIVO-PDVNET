using System;
using System.IO;
using ControleCaixa.Model.Interfaces;

namespace ControleCaixa.Data
{
    public class ConfiguracaoRepository : IConfiguracaoRepository
    {
        private readonly string _configPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "PDVnetControleCaixa", "alerta_config.txt");

        public decimal ObterLimiteAlerta(decimal valorPadrao = 100m)
        {
            try
            {
                if (File.Exists(_configPath) &&
                    decimal.TryParse(File.ReadAllText(_configPath), out decimal valorSalvo))
                {
                    return valorSalvo;
                }
            }
            catch (IOException) { /* arquivo corrompido/inacessível: cai no padrão abaixo */ }

            return valorPadrao;
        }

        public void SalvarLimiteAlerta(decimal valor)
        {
            var diretorio = Path.GetDirectoryName(_configPath);
            if (!string.IsNullOrEmpty(diretorio))
                Directory.CreateDirectory(diretorio);

            File.WriteAllText(_configPath, valor.ToString());
        }
    }
}
