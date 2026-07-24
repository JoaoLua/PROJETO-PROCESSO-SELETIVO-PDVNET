using System;
using ControleCaixa.Model.Enums;

namespace ControleCaixa.Model
{
    public class MovimentacaoCaixa
    {
        public int Id { get; set; }
        public required string Descricao { get; set; }
        public TipoMovimentacao Tipo { get; set; } 
        public required string Categoria { get; set; }
        public decimal Valor { get; set; }
        public DateTime DataMovimento { get; set; }
        public bool Status { get; set; }
    }
}
