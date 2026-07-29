using System;
using System.Threading.Tasks;
using ControleCaixa.Business.Services;
using ControleCaixa.Model.Interfaces;
using ControleCaixa.Model.DTOs;

namespace PDVnet.ControleCaixa.UI.ViewModels
{
    public class DashboardViewModel : BaseViewModel
    {
        private readonly IMovimentacaoService _service;

        private DashboardDTO _resumo;
        public DashboardDTO Resumo
        {
            get => _resumo;
            set
            {
                if (SetProperty(ref _resumo, value))
                {
                    OnPropertyChanged(nameof(IsSaldoNegativo));
                }
            }
        }

        public bool IsSaldoNegativo => Resumo?.SaldoTotal < 0;

        private decimal _limiteAlerta;
        public decimal LimiteAlerta
        {
            get => _limiteAlerta;
            set
            {
                if (SetProperty(ref _limiteAlerta, value))
                {
                    LimiteAlertaModificado?.Invoke(value);
                }
            }
        }

        public event Action<decimal> LimiteAlertaModificado;

        public DashboardViewModel(IMovimentacaoService service)
        {
            _service = service;
            
            _ = CarregarDadosAsync();
        }

        public async Task CarregarDadosAsync()
        {
            Resumo = await _service.ObterResumoDashboardAsync();
        }
    }
}
