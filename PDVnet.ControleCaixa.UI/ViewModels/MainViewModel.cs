using System;
using System.IO;
using System.Windows.Input;
using ControleCaixa.Business.Services;
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

        private readonly string _configPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "PDVnetControleCaixa", "alerta_config.txt");

        private decimal _limiteAlerta = 100m;
        public decimal LimiteAlerta
        {
            get => _limiteAlerta;
            set
            {
                if (SetProperty(ref _limiteAlerta, value))
                {
                    VerificarAlerta();
                    SalvarConfiguracao(value);
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
            CarregarConfiguracao();
            
            _service = new MovimentacaoService(new MovimentacaoRepository());

            AbrirMovimentacoesCommand = new RelayCommand(_ => AbrirMovimentacoes());
            AbrirDashboardCommand = new RelayCommand(_ => AbrirDashboard());
            
            AbrirMovimentacoes();
            
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

        private void CarregarConfiguracao()
        {
            try
            {
                if (File.Exists(_configPath))
                {
                    var conteudo = File.ReadAllText(_configPath);
                    if (decimal.TryParse(conteudo, out decimal valorSalvo))
                    {
                        _limiteAlerta = valorSalvo;
                    }
                }
            }
            catch { }
        }

        private void SalvarConfiguracao(decimal valor)
        {
            try
            {
                var diretorio = Path.GetDirectoryName(_configPath);
                if (!string.IsNullOrEmpty(diretorio))
                    Directory.CreateDirectory(diretorio);
                    
                File.WriteAllText(_configPath, valor.ToString());
            }
            catch { }
        }
    }
}
