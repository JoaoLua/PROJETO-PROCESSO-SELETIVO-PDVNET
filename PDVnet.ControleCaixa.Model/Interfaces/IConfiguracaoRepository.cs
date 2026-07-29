namespace ControleCaixa.Model.Interfaces
{
    public interface IConfiguracaoRepository
    {
        decimal ObterLimiteAlerta(decimal valorPadrao = 100m);
        void SalvarLimiteAlerta(decimal valor);
    }
}
