using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ControleCaixa.Model;
using ControleCaixa.Model.Enums;
using ControleCaixa.Model.Interfaces;
using ControleCaixa.Model.DTOs;
using ControleCaixa.Business.Validators;

namespace ControleCaixa.Business.Services
{
    public class MovimentacaoService : IMovimentacaoService
    {
        private readonly IMovimentacaoRepository _repository;
        private readonly MovimentacaoValidator _validator;

        public MovimentacaoService(IMovimentacaoRepository repository)
        {
            _repository = repository;
            _validator = new MovimentacaoValidator();
        }

        public async Task InserirAsync(MovimentacaoCaixa movimentacao)
        {
            var erros = _validator.Validar(movimentacao);
            if (erros.Any())
                throw new ArgumentException(string.Join("\n", erros));

            movimentacao.DataMovimento = DateTime.Now;

            movimentacao.Status = true; 
            
            await _repository.InserirAsync(movimentacao);
        }

        public async Task<List<MovimentacaoCaixa>> ListarAtivasAsync()
        {
            return await _repository.ListarAtivasAsync();
        }

        public async Task<List<MovimentacaoCaixa>> ListarPorFiltrosAsync(string texto, DateTime? dataInicio, DateTime? dataFim, string categoria = null, TipoMovimentacao? tipo = null, bool? ativo = true)
        {
            if (dataInicio.HasValue && dataFim.HasValue && dataInicio.Value.Date > dataFim.Value.Date)
                throw new ArgumentException("A data de início não pode ser maior que a data de fim.");

            return await _repository.ListarPorFiltrosAsync(texto, dataInicio, dataFim, categoria, tipo, ativo);
        }

        public async Task<MovimentacaoCaixa> BuscarPorIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Id inválido.");

            return await _repository.BuscarPorIdAsync(id);
        }

        public async Task AtualizarAsync(MovimentacaoCaixa movimentacao)
        {
            if (movimentacao.Id <= 0)
                throw new ArgumentException("Id inválido para atualização.");

            var erros = _validator.Validar(movimentacao);
            if (erros.Any())
                throw new ArgumentException(string.Join("\n", erros));

            await _repository.AtualizarAsync(movimentacao);
        }

        public async Task ExcluirAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("ID inválido para exclusão.");

            await _repository.ExcluirAsync(id);
        }

        public async Task ReativarAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("ID inválido para reativação.");

            await _repository.ReativarAsync(id);
        }

        public async Task<DashboardDTO> ObterResumoDashboardAsync()
        {
            return await _repository.ObterResumoDashboardAsync();
        }

        public async Task<bool> VerificarAlertaSaldoBaixoAsync(decimal limiteMinimo)
        {
            var resumo = await ObterResumoDashboardAsync();
            return resumo.SaldoTotal < limiteMinimo;
        }
    }
}
