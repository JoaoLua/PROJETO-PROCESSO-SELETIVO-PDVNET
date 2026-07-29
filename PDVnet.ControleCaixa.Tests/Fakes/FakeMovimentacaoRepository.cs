using ControleCaixa.Model;
using ControleCaixa.Model.Interfaces;
using ControleCaixa.Model.DTOs;
using ControleCaixa.Model.Enums;
using System.Threading.Tasks;

namespace PDVnet.ControleCaixa.Tests.Fakes
{
    public class FakeMovimentacaoRepository : IMovimentacaoRepository
    {
        private readonly List<MovimentacaoCaixa> _movimentacoes = new();
        private int _nextId = 1;

        public Task InserirAsync(MovimentacaoCaixa movimentacao)
        {
            movimentacao.Id = _nextId++;
            if (movimentacao.DataMovimento == default)
                movimentacao.DataMovimento = DateTime.Now;
            
            _movimentacoes.Add(movimentacao);
            return Task.CompletedTask;
        }

        public Task<List<MovimentacaoCaixa>> ListarAtivasAsync()
        {
            return Task.FromResult(_movimentacoes.Where(m => m.Status).ToList());
        }

        public Task<List<MovimentacaoCaixa>> ListarPorFiltrosAsync(string texto, DateTime? dataInicio, DateTime? dataFim, string categoria = null, TipoMovimentacao? tipo = null, bool? ativo = true)
        {
            var query = _movimentacoes.AsEnumerable();

            if (ativo.HasValue)
                query = query.Where(m => m.Status == ativo.Value);

            if (!string.IsNullOrWhiteSpace(texto))
                query = query.Where(m => m.Descricao.Contains(texto, StringComparison.OrdinalIgnoreCase));
            
            if (dataInicio.HasValue)
                query = query.Where(m => m.DataMovimento >= dataInicio.Value);
            
            if (dataFim.HasValue)
                query = query.Where(m => m.DataMovimento <= dataFim.Value.AddDays(1).AddTicks(-1));

            if (!string.IsNullOrWhiteSpace(categoria))
                query = query.Where(m => m.Categoria == categoria);

            if (tipo.HasValue)
                query = query.Where(m => m.Tipo == tipo.Value);

            return Task.FromResult(query.ToList());
        }

        public Task<MovimentacaoCaixa> BuscarPorIdAsync(int id)
        {
            return Task.FromResult(_movimentacoes.FirstOrDefault(m => m.Id == id));
        }

        public async Task AtualizarAsync(MovimentacaoCaixa movimentacao)
        {
            var existente = await BuscarPorIdAsync(movimentacao.Id);
            if (existente != null)
            {
                existente.Descricao = movimentacao.Descricao;
                existente.Valor = movimentacao.Valor;
                existente.Tipo = movimentacao.Tipo;
                existente.CategoriaId = movimentacao.CategoriaId;
                existente.Categoria = movimentacao.Categoria;
                existente.DataMovimento = movimentacao.DataMovimento;
            }
        }

        public async Task ExcluirAsync(int id)
        {
            var existente = await BuscarPorIdAsync(id);
            if (existente != null)
                existente.Status = false;
        }

        public async Task ReativarAsync(int id)
        {
            var existente = await BuscarPorIdAsync(id);
            if (existente != null)
                existente.Status = true;
        }

        public Task<DashboardDTO> ObterResumoDashboardAsync()
        {
            var ativas = _movimentacoes.Where(m => m.Status).ToList();
            
            var dto = new DashboardDTO
            {
                TotalMovimentacoes = ativas.Count,
                TotalEntradas = ativas.Where(m => m.Tipo == TipoMovimentacao.Entrada).Sum(m => m.Valor),
                TotalSaidas = ativas.Where(m => m.Tipo == TipoMovimentacao.Saida).Sum(m => m.Valor)
            };
            
            dto.SaldoTotal = dto.TotalEntradas - dto.TotalSaidas;
            return Task.FromResult(dto);
        }
    }
}
