using System.Windows.Input;
using ControleCaixa.Bussines.Services;
using ControleCaixa.Data;

namespace PDVnet.ControleCaixa.UI.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly MovimentacaoService _service;

        private BaseViewModel _currentViewModel;
        public BaseViewModel CurrentViewModel
        {
            get => _currentViewModel;
            set => SetProperty(ref _currentViewModel, value);
        }

        private decimal _limiteAlerta = 100m;
        public decimal LimiteAlerta
        {
            get => _limiteAlerta;
            set
            {
                if (SetProperty(ref _limiteAlerta, value))
                {
                    VerificarAlerta();
                }
            }
        }

        private bool _mostrarAlerta;
        public bool MostrarAlerta
        {
            get => _mostrarAlerta;
            set => SetProperty(ref _mostrarAlerta, value);
        }

        private string _mensagemAlerta;
        public string MensagemAlerta
        {
            get => _mensagemAlerta;
            set => SetProperty(ref _mensagemAlerta, value);
        }

        private decimal _saldoAtual;

        public ICommand AbrirMovimentacoesCommand { get; }
        public ICommand AbrirDashboardCommand { get; }

        public MainViewModel()
        {
            _service = new MovimentacaoService(new MovimentacaoRepository());

            AbrirMovimentacoesCommand = new RelayCommand(_ => AbrirMovimentacoes());
            AbrirDashboardCommand = new RelayCommand(_ => AbrirDashboard());
            
            // Inicia nas Movimentações
            AbrirMovimentacoes();
            
            // Garante que o alerta do cabeçalho seja calculado logo na inicialização
            AtualizarResumo();
        }

        private DashboardViewModel _dashboardViewModel;
        private MovimentacoesViewModel _movimentacoesViewModel;

        private void AbrirMovimentacoes()
        {
            if (_movimentacoesViewModel == null)
            {
                _movimentacoesViewModel = new MovimentacoesViewModel(() => AtualizarResumo());
            }
            else
            {
                // Recarrega os dados caso tenha havido mudanças via dashboard
                _movimentacoesViewModel.BuscarCommand.Execute(null);
            }
            
            CurrentViewModel = _movimentacoesViewModel;
        }

        private void AbrirDashboard()
        {
            if (_dashboardViewModel == null)
            {
                _dashboardViewModel = new DashboardViewModel(this, _service);
            }
            else
            {
                _dashboardViewModel.CarregarDados();
            }

            AtualizarResumo();
            CurrentViewModel = _dashboardViewModel;
        }

        public void AtualizarResumo()
        {
            var resumo = _service.ObterResumoDashboard();
            _saldoAtual = resumo.SaldoTotal;
            VerificarAlerta();
        }

        private void VerificarAlerta()
        {
            if (_saldoAtual < LimiteAlerta)
            {
                MensagemAlerta = $"Alerta: O saldo atual ({_saldoAtual:C}) está baixo!";
                MostrarAlerta = true;
            }
            else
            {
                MostrarAlerta = false;
            }
        }
    }
}
