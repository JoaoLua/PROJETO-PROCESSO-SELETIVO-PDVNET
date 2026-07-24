using ControleCaixa.Bussines.Services;
using ControleCaixa.Model.DTOs;

namespace PDVnet.ControleCaixa.UI.ViewModels
{
    public class DashboardViewModel : BaseViewModel
    {
        private readonly MainViewModel _mainViewModel;
        private readonly MovimentacaoService _service;

        private DashboardDTO _resumo;
        public DashboardDTO Resumo
        {
            get => _resumo;
            set => SetProperty(ref _resumo, value);
        }

        public decimal LimiteAlerta
        {
            get => _mainViewModel.LimiteAlerta;
            set
            {
                if (_mainViewModel.LimiteAlerta != value)
                {
                    _mainViewModel.LimiteAlerta = value;
                    OnPropertyChanged(nameof(LimiteAlerta));
                }
            }
        }

        public DashboardViewModel(MainViewModel mainViewModel, MovimentacaoService service)
        {
            _mainViewModel = mainViewModel;
            _service = service;
            
            CarregarDados();
        }

        public void CarregarDados()
        {
            Resumo = _service.ObterResumoDashboard();
        }
    }
}
