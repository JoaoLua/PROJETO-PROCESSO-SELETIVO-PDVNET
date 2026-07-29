using System;
using System.Threading.Tasks;
using System.Windows.Input;
using ControleCaixa.Business.Services;
using ControleCaixa.Model.Interfaces;

namespace PDVnet.ControleCaixa.UI.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private BaseViewModel _currentViewModel;
        public BaseViewModel CurrentViewModel
        {
            get => _currentViewModel;
            set
            {
                if (SetProperty(ref _currentViewModel, value))
                {
                    OnPropertyChanged(nameof(IsMovimentacoesSelected));
                    OnPropertyChanged(nameof(IsDashboardSelected));
                    OnPropertyChanged(nameof(IsCategoriasSelected));
                }
            }
        }

        public bool IsMovimentacoesSelected => CurrentViewModel is MovimentacoesViewModel;
        public bool IsDashboardSelected => CurrentViewModel is DashboardViewModel;
        public bool IsCategoriasSelected => CurrentViewModel is CategoriasViewModel;

        private readonly IConfiguracaoRepository _configuracaoRepository;

        private decimal _limiteAlerta = 100m;
        public decimal LimiteAlerta
        {
            get => _limiteAlerta;
            set
            {
                if (SetProperty(ref _limiteAlerta, value))
                {
                    VerificarAlerta();
                    try
                    {
                        _configuracaoRepository.SalvarLimiteAlerta(value);
                    }
                    catch (Exception)
                    {
                        MensagemAlerta = "Não foi possível salvar sua preferência de alerta.";
                        MostrarAlerta = true;
                    }
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

        private readonly IMovimentacaoService _service;
        private readonly ICategoriaService _categoriaService;

        public ICommand AbrirMovimentacoesCommand { get; }
        public ICommand AbrirDashboardCommand { get; }
        public ICommand AbrirCategoriasCommand { get; }

        private DashboardViewModel _dashboardViewModel;
        private MovimentacoesViewModel _movimentacoesViewModel;
        private CategoriasViewModel _categoriasViewModel;

        public MainViewModel(
            IMovimentacaoService service,
            ICategoriaService categoriaService,
            IConfiguracaoRepository configuracaoRepository,
            DashboardViewModel dashboardViewModel,
            MovimentacoesViewModel movimentacoesViewModel,
            CategoriasViewModel categoriasViewModel)
        {
            _service = service;
            _categoriaService = categoriaService;
            _configuracaoRepository = configuracaoRepository;

            _dashboardViewModel = dashboardViewModel;
            _dashboardViewModel.LimiteAlerta = this.LimiteAlerta;
            _dashboardViewModel.LimiteAlertaModificado += (val) => this.LimiteAlerta = val;

            _movimentacoesViewModel = movimentacoesViewModel;
            _movimentacoesViewModel.OnMovimentacaoSaved += () => _ = AtualizarResumoAsync();

            _categoriasViewModel = categoriasViewModel;

            _limiteAlerta = _configuracaoRepository.ObterLimiteAlerta();
            if (_dashboardViewModel != null)
                _dashboardViewModel.LimiteAlerta = _limiteAlerta;

            AbrirMovimentacoesCommand = new RelayCommand(_ => AbrirMovimentacoes());
            AbrirDashboardCommand = new RelayCommand(async _ => await AbrirDashboardAsync());
            AbrirCategoriasCommand = new RelayCommand(_ => AbrirCategorias());
            
            AbrirMovimentacoes();
            
            _ = AtualizarResumoAsync();
        }

        private void AbrirMovimentacoes()
        {
            _movimentacoesViewModel.BuscarCommand.Execute(null);
            CurrentViewModel = _movimentacoesViewModel;
        }

        private async Task AbrirDashboardAsync()
        {
            await _dashboardViewModel.CarregarDadosAsync();
            await AtualizarResumoAsync();
            CurrentViewModel = _dashboardViewModel;
        }

        private void AbrirCategorias()
        {
            CurrentViewModel = _categoriasViewModel;
        }

        public async Task AtualizarResumoAsync()
        {
            var resumo = await _service.ObterResumoDashboardAsync();
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
