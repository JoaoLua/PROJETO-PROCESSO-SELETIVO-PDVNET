using ControleCaixa.Model;
using ControleCaixa.Model.Interfaces;
using ControleCaixa.Model.DTOs;
using ControleCaixa.Model.Enums;

namespace PDVnet.ControleCaixa.Tests.Fakes
{
    public class FakeMovimentacaoRepository : IMovimentacaoRepository
    {
        private readonly List<MovimentacaoCaixa> _movimentacoes = new();
        private int _nextId = 1;

        public void Inserir(MovimentacaoCaixa movimentacao)
        {
            movimentacao.Id = _nextId++;
            if (movimentacao.DataMovimento == default)
                movimentacao.DataMovimento = DateTime.Now;
            
            _movimentacoes.Add(movimentacao);
        }

        public List<MovimentacaoCaixa> ListarAtivas()
        {
            return _movimentacoes.Where(m => m.Status).ToList();
        }

        public List<MovimentacaoCaixa> ListarPorFiltros(string texto, DateTime? dataInicio, DateTime? dataFim)
        {
            var query = _movimentacoes.Where(m => m.Status);

            if (!string.IsNullOrWhiteSpace(texto))
                query = query.Where(m => m.Descricao.Contains(texto, StringComparison.OrdinalIgnoreCase));
            
            if (dataInicio.HasValue)
                query = query.Where(m => m.DataMovimento >= dataInicio.Value);
            
            if (dataFim.HasValue)
                query = query.Where(m => m.DataMovimento <= dataFim.Value.AddDays(1).AddTicks(-1));

            return query.ToList();
        }

        public MovimentacaoCaixa BuscarPorId(int id)
        {
            return _movimentacoes.FirstOrDefault(m => m.Id == id);
        }

        public void Atualizar(MovimentacaoCaixa movimentacao)
        {
            var existente = BuscarPorId(movimentacao.Id);
            if (existente != null)
            {
                existente.Descricao = movimentacao.Descricao;
                existente.Valor = movimentacao.Valor;
                existente.Tipo = movimentacao.Tipo;
                existente.Categoria = movimentacao.Categoria;
                existente.DataMovimento = movimentacao.DataMovimento;
            }
        }

        public void Excluir(int id)
        {
            var existente = BuscarPorId(id);
            if (existente != null)
                existente.Status = false;
        }

        public DashboardDTO ObterResumoDashboard()
        {
            var ativas = _movimentacoes.Where(m => m.Status).ToList();
            
            var dto = new DashboardDTO
            {
                TotalMovimentacoes = ativas.Count,
                TotalEntradas = ativas.Where(m => m.Tipo == TipoMovimentacao.Entrada).Sum(m => m.Valor),
                TotalSaidas = ativas.Where(m => m.Tipo == TipoMovimentacao.Saida).Sum(m => m.Valor)
            };
            
            dto.SaldoTotal = dto.TotalEntradas - dto.TotalSaidas;
            return dto;
        }
    }
}
