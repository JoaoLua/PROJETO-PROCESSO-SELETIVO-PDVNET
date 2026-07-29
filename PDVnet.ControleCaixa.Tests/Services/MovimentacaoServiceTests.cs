using System;
using System.Threading.Tasks;
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
        public async Task ObterResumoDashboard_DeveCalcularSaldoCorretamente()
        {

            await _service.InserirAsync(new MovimentacaoCaixa { Descricao = "Venda", Valor = 100, Tipo = TipoMovimentacao.Entrada, Categoria = "" });
            await _service.InserirAsync(new MovimentacaoCaixa { Descricao = "Pagamento", Valor = 30, Tipo = TipoMovimentacao.Saida, Categoria = "" });
            await _service.InserirAsync(new MovimentacaoCaixa { Descricao = "Venda 2", Valor = 50, Tipo = TipoMovimentacao.Entrada, Categoria = "" });

            var resumo = await _service.ObterResumoDashboardAsync();

            Assert.AreEqual(150m, resumo.TotalEntradas);
            Assert.AreEqual(30m, resumo.TotalSaidas);
            Assert.AreEqual(120m, resumo.SaldoTotal);
        }

        [TestMethod]
        public async Task VerificarAlertaSaldoBaixo_DeveRetornarTrue_QuandoAbaixoDoMinimo()
        {

            await _service.InserirAsync(new MovimentacaoCaixa { Descricao = "Venda", Valor = 100, Tipo = TipoMovimentacao.Entrada, Categoria = "" });
            await _service.InserirAsync(new MovimentacaoCaixa { Descricao = "Saida", Valor = 90, Tipo = TipoMovimentacao.Saida, Categoria = "" });


            bool disparouAlerta = await _service.VerificarAlertaSaldoBaixoAsync(50m); 

            Assert.IsTrue(disparouAlerta);
        }

        [TestMethod]
        public async Task VerificarAlertaSaldoBaixo_DeveRetornarFalse_QuandoAcimaDoMinimo()
        {

            await _service.InserirAsync(new MovimentacaoCaixa { Descricao = "Venda", Valor = 100, Tipo = TipoMovimentacao.Entrada, Categoria = "" });


            bool disparouAlerta = await _service.VerificarAlertaSaldoBaixoAsync(50m); 

            Assert.IsFalse(disparouAlerta);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task ListarPorFiltros_DataInicioMaiorQueDataFim_DeveLancarExcecao()
        {

            var dataInicio = new DateTime(2023, 12, 31);
            var dataFim = new DateTime(2023, 01, 01);

            await _service.ListarPorFiltrosAsync("", dataInicio, dataFim);
        }

        [TestMethod]
        public async Task Inserir_DeveAtribuirDataMovimentoAutomaticamente()
        {
            var mov = new MovimentacaoCaixa { Descricao = "Venda", Valor = 50, Tipo = TipoMovimentacao.Entrada };

            await _service.InserirAsync(mov);

            Assert.AreNotEqual(default(DateTime), mov.DataMovimento, "A data deve ser gerada automaticamente.");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task Inserir_ValorNegativo_DeveLancarExcecao()
        {
            var mov = new MovimentacaoCaixa { Descricao = "Teste", Valor = -10, Tipo = TipoMovimentacao.Entrada };

            await _service.InserirAsync(mov);
        }

        [TestMethod]
        public async Task Atualizar_DeveModificarDescricao()
        {
            var mov = new MovimentacaoCaixa { Descricao = "Original", Valor = 100, Tipo = TipoMovimentacao.Entrada };
            await _service.InserirAsync(mov);

            mov.Descricao = "Alterada";
            await _service.AtualizarAsync(mov);
            var atualizada = await _service.BuscarPorIdAsync(mov.Id);

            Assert.AreEqual("Alterada", atualizada.Descricao);
        }

        [TestMethod]
        public async Task Excluir_DeveRemoverDaListaDeAtivas()
        {
            var mov = new MovimentacaoCaixa { Descricao = "Para Excluir", Valor = 50, Tipo = TipoMovimentacao.Saida };
            await _service.InserirAsync(mov);
            Assert.AreEqual(1, (await _service.ListarAtivasAsync()).Count);

            await _service.ExcluirAsync(mov.Id);

            Assert.AreEqual(0, (await _service.ListarAtivasAsync()).Count, "A movimentação excluída não deve aparecer nas ativas.");
        }

        [TestMethod]
        public async Task BuscarPorId_DeveRetornarMovimentacaoCorreta()
        {
            await _service.InserirAsync(new MovimentacaoCaixa { Descricao = "Primeira", Valor = 10, Tipo = TipoMovimentacao.Entrada });
            await _service.InserirAsync(new MovimentacaoCaixa { Descricao = "Segunda", Valor = 20, Tipo = TipoMovimentacao.Saida });

            var resultado = await _service.BuscarPorIdAsync(2);

            Assert.IsNotNull(resultado);
            Assert.AreEqual("Segunda", resultado.Descricao);
            Assert.AreEqual(20m, resultado.Valor);
        }
    }
}
