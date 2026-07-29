using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ControleCaixa.Model.DTOs;

namespace ControleCaixa.Model.Interfaces
{
    public interface IMovimentacaoService
    {
        Task InserirAsync(MovimentacaoCaixa movimentacao);
        Task<List<MovimentacaoCaixa>> ListarAtivasAsync();
        Task<List<MovimentacaoCaixa>> ListarPorFiltrosAsync(string texto, DateTime? dataInicio, DateTime? dataFim, string categoria = null, Enums.TipoMovimentacao? tipo = null, bool? ativo = true);
        Task<bool> VerificarAlertaSaldoBaixoAsync(decimal limiteMinimo);
        Task<MovimentacaoCaixa> BuscarPorIdAsync(int id);
        Task AtualizarAsync(MovimentacaoCaixa movimentacao);
        Task ExcluirAsync(int id);
        Task ReativarAsync(int id);
        
        Task<DashboardDTO> ObterResumoDashboardAsync();
    }
}
