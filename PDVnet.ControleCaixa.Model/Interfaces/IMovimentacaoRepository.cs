using System.Collections.Generic;
using ControleCaixa.Model.DTOs;

namespace ControleCaixa.Model.Interfaces
{
    public interface IMovimentacaoRepository
    {
        void Inserir(MovimentacaoCaixa movimentacao);
        List<MovimentacaoCaixa> ListarAtivas();
        List<MovimentacaoCaixa> ListarPorFiltros(string texto, System.DateTime? dataInicio, System.DateTime? dataFim);
        MovimentacaoCaixa BuscarPorId(int id);
        void Atualizar(MovimentacaoCaixa movimentacao);
        void Excluir(int id);
        
        DashboardDTO ObterResumoDashboard();
    }
}
