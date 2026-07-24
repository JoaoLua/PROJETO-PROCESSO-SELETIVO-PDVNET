using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ControleCaixa.Model;
using ControleCaixa.Model.Enums;
using ControleCaixa.Bussines.Validators;

namespace PDVnet.ControleCaixa.Tests.Validators
{
    [TestClass]
    public class MovimentacaoValidatorTests
    {
        private MovimentacaoValidator _validator;

        [TestInitialize]
        public void Setup()
        {
            _validator = new MovimentacaoValidator();
        }

        [TestMethod]
        public void Validar_DescricaoNula_DeveRetornarErro()
        {
            // Arrange
            var mov = new MovimentacaoCaixa 
            { 
                Descricao = null,
                Valor = 50,
                Tipo = TipoMovimentacao.Entrada,
                Categoria = ""
            };

            // Act
            var erros = _validator.Validar(mov);

            // Assert
            Assert.IsTrue(erros.Any(e => e.Contains("obrigatória")));
        }

        [TestMethod]
        public void Validar_DescricaoVazia_DeveRetornarErro()
        {
            // Arrange
            var mov = new MovimentacaoCaixa 
            { 
                Descricao = "   ",
                Valor = 50,
                Tipo = TipoMovimentacao.Entrada,
                Categoria = ""
            };

            // Act
            var erros = _validator.Validar(mov);

            // Assert
            Assert.IsTrue(erros.Any(e => e.Contains("obrigatória")));
        }

        [TestMethod]
        public void Validar_ValorZero_DeveRetornarErro()
        {
            // Arrange
            var mov = new MovimentacaoCaixa 
            { 
                Descricao = "Teste",
                Valor = 0,
                Tipo = TipoMovimentacao.Entrada,
                Categoria = ""
            };

            // Act
            var erros = _validator.Validar(mov);

            // Assert
            Assert.IsTrue(erros.Any(e => e.Contains("não pode ser negativo ou igual a zero")));
        }

        [TestMethod]
        public void Validar_ValorNegativo_DeveRetornarErro()
        {
            // Arrange
            var mov = new MovimentacaoCaixa 
            { 
                Descricao = "Teste",
                Valor = -10,
                Tipo = TipoMovimentacao.Saida,
                Categoria = ""
            };

            // Act
            var erros = _validator.Validar(mov);

            // Assert
            Assert.IsTrue(erros.Any(e => e.Contains("não pode ser negativo ou igual a zero")));
        }

        [TestMethod]
        public void Validar_MovimentacaoValida_NaoDeveRetornarErros()
        {
            // Arrange
            var mov = new MovimentacaoCaixa 
            { 
                Descricao = "Venda de Produto",
                Valor = 150.50m,
                Tipo = TipoMovimentacao.Entrada,
                Categoria = "Vendas"
            };

            // Act
            var erros = _validator.Validar(mov);

            // Assert
            Assert.AreEqual(0, erros.Count, "Não deveria retornar erros para uma movimentação válida.");
        }
    }
}
