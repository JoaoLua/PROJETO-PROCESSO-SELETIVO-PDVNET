using System;
using System.Collections.Generic;
using ControleCaixa.Model;
using ControleCaixa.Model.Enums;

namespace ControleCaixa.Business.Validators
{
    public class MovimentacaoValidator
    {
        public List<string> Validar(MovimentacaoCaixa movimentacao)
        {
            var erros = new List<string>();

            if (movimentacao == null)
            {
                erros.Add("Movimentação não pode ser nula.");
                return erros;
            }

            if (string.IsNullOrWhiteSpace(movimentacao.Descricao))
                erros.Add("A descrição é obrigatória.");
            else if (movimentacao.Descricao.Length > 200)
                erros.Add("A descrição deve ter no máximo 200 caracteres.");

            if (movimentacao.Valor <= 0)
                erros.Add("O Valor não pode ser negativo ou igual a zero; o sinal da movimentação é definido exclusivamente pelo campo Tipo.");
            else if (movimentacao.Valor > 99999999.99m)
                erros.Add("O Valor excede o limite máximo permitido de 99.999.999,99.");

            if (!Enum.IsDefined(typeof(TipoMovimentacao), movimentacao.Tipo))
                erros.Add("O tipo de movimentação é inválido.");



            return erros;
        }
    }
}
