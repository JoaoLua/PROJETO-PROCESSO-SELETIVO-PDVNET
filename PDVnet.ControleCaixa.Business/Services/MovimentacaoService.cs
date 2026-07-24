using System;
using System.Collections.Generic;
using System.Linq;
using ControleCaixa.Model;
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

        public void Inserir(MovimentacaoCaixa movimentacao)
        {
            var erros = _validator.Validar(movimentacao);
            if (erros.Any())
                throw new ArgumentException(string.Join("\n", erros));

            // A data/hora do lançamento deve ser gerada automaticamente no momento da criação
            movimentacao.DataMovimento = DateTime.Now;

            movimentacao.Status = true; // Força ativo na criação
            
            _repository.Inserir(movimentacao);
        }

        public List<MovimentacaoCaixa> ListarAtivas()
        {
            return _repository.ListarAtivas();
        }

        public List<MovimentacaoCaixa> ListarPorFiltros(string texto, DateTime? dataInicio, DateTime? dataFim)
        {
            if (dataInicio.HasValue && dataFim.HasValue && dataInicio.Value.Date > dataFim.Value.Date)
                throw new ArgumentException("A data de início não pode ser maior que a data de fim.");

            return _repository.ListarPorFiltros(texto, dataInicio, dataFim);
        }

        public MovimentacaoCaixa BuscarPorId(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Id inválido.");

            return _repository.BuscarPorId(id);
        }

        public void Atualizar(MovimentacaoCaixa movimentacao)
        {
            if (movimentacao.Id <= 0)
                throw new ArgumentException("Id inválido para atualização.");

            var erros = _validator.Validar(movimentacao);
            if (erros.Any())
                throw new ArgumentException(string.Join("\n", erros));

            _repository.Atualizar(movimentacao);
        }

        public void Excluir(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Id inválido para exclusão.");

            _repository.Excluir(id);
        }

        public DashboardDTO ObterResumoDashboard()
        {
            return _repository.ObterResumoDashboard();
        }

        public bool VerificarAlertaSaldoBaixo(decimal limiteMinimo)
        {
            var resumo = ObterResumoDashboard();
            return resumo.SaldoTotal < limiteMinimo;
        }
    }
}
