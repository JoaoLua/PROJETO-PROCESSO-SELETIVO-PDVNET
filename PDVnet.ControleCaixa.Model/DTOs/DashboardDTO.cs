namespace ControleCaixa.Model.DTOs
{
    public class DashboardDTO
    {
        public decimal SaldoTotal { get; set; }
        public int TotalMovimentacoes { get; set; }
        public decimal TotalEntradas { get; set; }
        public decimal TotalSaidas { get; set; }
    }
}
