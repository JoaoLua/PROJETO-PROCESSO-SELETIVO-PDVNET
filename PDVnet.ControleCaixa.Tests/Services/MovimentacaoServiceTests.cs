using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ControleCaixa.Model;
using ControleCaixa.Model.Enums;
using ControleCaixa.Business.Services;
using PDVnet.ControleCaixa.Tests.Fakes;

namespace PDVnet.ControleCaixa.Tests.Services
{
    [TestClass]
    public class MovimentacaoServiceTests
    {
        private FakeMovimentacaoRepository _repository;
        private MovimentacaoService _service;

        [TestInitialize]
        public void Setup()
        {
            _repository = new FakeMovimentacaoRepository();
            _service = new MovimentacaoService(_repository);
        }

        [TestMethod]
        public void ObterResumoDashboard_DeveCalcularSaldoCorretamente()
        {

            _service.Inserir(new MovimentacaoCaixa { Descricao = "Venda", Valor = 100, Tipo = TipoMovimentacao.Entrada, Categoria = "" });
            _service.Inserir(new MovimentacaoCaixa { Descricao = "Pagamento", Valor = 30, Tipo = TipoMovimentacao.Saida, Categoria = "" });
            _service.Inserir(new MovimentacaoCaixa { Descricao = "Venda 2", Valor = 50, Tipo = TipoMovimentacao.Entrada, Categoria = "" });

            var resumo = _service.ObterResumoDashboard();

            Assert.AreEqual(150m, resumo.TotalEntradas);
            Assert.AreEqual(30m, resumo.TotalSaidas);
            Assert.AreEqual(120m, resumo.SaldoTotal);
        }

        [TestMethod]
        public void VerificarAlertaSaldoBaixo_DeveRetornarTrue_QuandoAbaixoDoMinimo()
        {

            _service.Inserir(new MovimentacaoCaixa { Descricao = "Venda", Valor = 100, Tipo = TipoMovimentacao.Entrada, Categoria = "" });
            _service.Inserir(new MovimentacaoCaixa { Descricao = "Saida", Valor = 90, Tipo = TipoMovimentacao.Saida, Categoria = "" });


            bool disparouAlerta = _service.VerificarAlertaSaldoBaixo(50m); 

            Assert.IsTrue(disparouAlerta);
        }

        [TestMethod]
        public void VerificarAlertaSaldoBaixo_DeveRetornarFalse_QuandoAcimaDoMinimo()
        {

            _service.Inserir(new MovimentacaoCaixa { Descricao = "Venda", Valor = 100, Tipo = TipoMovimentacao.Entrada, Categoria = "" });


            bool disparouAlerta = _service.VerificarAlertaSaldoBaixo(50m); 

            Assert.IsFalse(disparouAlerta);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ListarPorFiltros_DataInicioMaiorQueDataFim_DeveLancarExcecao()
        {

            var dataInicio = new DateTime(2023, 12, 31);
            var dataFim = new DateTime(2023, 01, 01);

            _service.ListarPorFiltros("", dataInicio, dataFim);
        }

        [TestMethod]
        public void Inserir_DeveAtribuirDataMovimentoAutomaticamente()
        {
            var mov = new MovimentacaoCaixa { Descricao = "Venda", Valor = 50, Tipo = TipoMovimentacao.Entrada };

            _service.Inserir(mov);

            Assert.AreNotEqual(default(DateTime), mov.DataMovimento, "A data deve ser gerada automaticamente.");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Inserir_ValorNegativo_DeveLancarExcecao()
        {
            var mov = new MovimentacaoCaixa { Descricao = "Teste", Valor = -10, Tipo = TipoMovimentacao.Entrada };

            _service.Inserir(mov);
        }

        [TestMethod]
        public void Atualizar_DeveModificarDescricao()
        {
            var mov = new MovimentacaoCaixa { Descricao = "Original", Valor = 100, Tipo = TipoMovimentacao.Entrada };
            _service.Inserir(mov);

            mov.Descricao = "Alterada";
            _service.Atualizar(mov);
            var atualizada = _service.BuscarPorId(mov.Id);

            Assert.AreEqual("Alterada", atualizada.Descricao);
        }

        [TestMethod]
        public void Excluir_DeveRemoverDaListaDeAtivas()
        {
            var mov = new MovimentacaoCaixa { Descricao = "Para Excluir", Valor = 50, Tipo = TipoMovimentacao.Saida };
            _service.Inserir(mov);
            Assert.AreEqual(1, _service.ListarAtivas().Count);

            _service.Excluir(mov.Id);

            Assert.AreEqual(0, _service.ListarAtivas().Count, "A movimentação excluída não deve aparecer nas ativas.");
        }

        [TestMethod]
        public void BuscarPorId_DeveRetornarMovimentacaoCorreta()
        {
            _service.Inserir(new MovimentacaoCaixa { Descricao = "Primeira", Valor = 10, Tipo = TipoMovimentacao.Entrada });
            _service.Inserir(new MovimentacaoCaixa { Descricao = "Segunda", Valor = 20, Tipo = TipoMovimentacao.Saida });

            var resultado = _service.BuscarPorId(2);

            Assert.IsNotNull(resultado);
            Assert.AreEqual("Segunda", resultado.Descricao);
            Assert.AreEqual(20m, resultado.Valor);
        }
    }
}
