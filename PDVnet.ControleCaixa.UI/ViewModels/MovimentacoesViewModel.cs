using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Threading;
using ControleCaixa.Model;
using ControleCaixa.Model.Enums;
using ControleCaixa.Model.Interfaces;
using ControleCaixa.Business.Services;
using ControleCaixa.Data;
using MaterialDesignThemes.Wpf;
using PDVnet.ControleCaixa.UI.Views;
using System.Linq;
using System.Threading.Tasks;

namespace PDVnet.ControleCaixa.UI.ViewModels
{
    public class MovimentacoesViewModel : BaseViewModel
    {
        private readonly IMovimentacaoService _service;
        private readonly ICategoriaService _categoriaService;

        public event System.Action OnMovimentacaoSaved;

        private readonly DispatcherTimer _debounceTimer;
        
        private ObservableCollection<MovimentacaoCaixa> _movimentacoes;
        public ObservableCollection<MovimentacaoCaixa> Movimentacoes
        {
            get => _movimentacoes;
            set => SetProperty(ref _movimentacoes, value);
        }

        private string _textoBusca;
        public string TextoBusca
        {
            get => _textoBusca;
            set 
            { 
                if (SetProperty(ref _textoBusca, value))
                {
                    ReiniciarDebounce();
                }
            }
        }

        private DateTime? _dataInicio;
        public DateTime? DataInicio
        {
            get => _dataInicio;
            set
            {
                if (SetProperty(ref _dataInicio, value))
                {
                    ReiniciarDebounce();
                }
            }
        }

        private DateTime? _dataFim;
        public DateTime? DataFim
        {
            get => _dataFim;
            set
            {
                if (SetProperty(ref _dataFim, value))
                {
                    ReiniciarDebounce();
                }
            }
        }

        private bool _somenteInativos;
        public bool SomenteInativos
        {
            get => _somenteInativos;
            set
            {
                if (SetProperty(ref _somenteInativos, value))
                {
                    ReiniciarDebounce();
                }
            }
        }


        private string _mensagemErroFiltro;
        public string MensagemErroFiltro
        {
            get => _mensagemErroFiltro;
            set 
            {
                if (SetProperty(ref _mensagemErroFiltro, value))
                {
                    OnPropertyChanged(nameof(TemErroFiltro));
                }
            }
        }

        public bool TemErroFiltro => !string.IsNullOrWhiteSpace(MensagemErroFiltro);

        public List<string> CategoriasDisponiveis { get; private set; }

        public List<string> TiposDisponiveis { get; } = new List<string>
        {
            "Todos",
            "Entrada",
            "Saída"
        };

        private string _categoriaSelecionada = "Todas";
        public string CategoriaSelecionada
        {
            get => _categoriaSelecionada;
            set
            {
                if (SetProperty(ref _categoriaSelecionada, value))
                {
                    ReiniciarDebounce();
                }
            }
        }

        private string _tipoSelecionado = "Todos";
        public string TipoSelecionado
        {
            get => _tipoSelecionado;
            set
            {
                if (SetProperty(ref _tipoSelecionado, value))
                {
                    ReiniciarDebounce();
                }
            }
        }

        public ICommand BuscarCommand { get; }
        public ICommand EditarCommand { get; }
        public ICommand DeletarCommand { get; }
        public ICommand ReativarCommand { get; }
        public ICommand NovaMovimentacaoCommand { get; }

        public MovimentacoesViewModel(IMovimentacaoService service, ICategoriaService categoriaService)
        {
            _service = service;
            _categoriaService = categoriaService;
            
            _ = CarregarFiltroCategoriasAsync();

            _debounceTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(400)
            };
            _debounceTimer.Tick += async (s, e) =>
            {
                _debounceTimer.Stop();
                await CarregarMovimentacoesAsync();
            };
            
            BuscarCommand = new RelayCommand(async _ => await CarregarMovimentacoesAsync());
            
            NovaMovimentacaoCommand = new RelayCommand(async _ => await AbrirModalMovimentacao());
            EditarCommand = new RelayCommand(async parametro => await EditarMovimentacao(parametro));
            
            DeletarCommand = new RelayCommand(async parametro => await DeletarMovimentacaoAsync(parametro));
            ReativarCommand = new RelayCommand(async parametro => await ReativarMovimentacaoAsync(parametro));
            
            _ = CarregarMovimentacoesAsync();
        }

        private void ReiniciarDebounce()
        {
            _debounceTimer.Stop();
            _debounceTimer.Start();
        }

        private async Task CarregarFiltroCategoriasAsync()
        {
            var categoriasDoBanco = await _categoriaService.ListarTodasAsync();
            var lista = new List<string> { "Todas" };
            lista.AddRange(categoriasDoBanco.Select(c => c.Nome));
            CategoriasDisponiveis = lista;
            OnPropertyChanged(nameof(CategoriasDisponiveis));
        }

        private async Task AbrirModalMovimentacao(MovimentacaoCaixa movimentacaoExistente = null)
        {
            var formVm = new MovimentacaoFormViewModel(_categoriaService, movimentacaoExistente);
            var formView = new MovimentacaoFormView { DataContext = formVm };

            var result = await DialogHost.Show(formView, "RootDialog");

            if (result is MovimentacaoCaixa mov)
            {
                if (mov.Id == 0)
                {
                    await _service.InserirAsync(mov);
                }
                else
                {
                    await _service.AtualizarAsync(mov);
                }
                await CarregarMovimentacoesAsync(); 
                OnMovimentacaoSaved?.Invoke();
            }
        }

        private async Task EditarMovimentacao(object parametro)
        {
            if (parametro is MovimentacaoCaixa mov)
            {
                var clone = new MovimentacaoCaixa 
                {
                    Id = mov.Id,
                    Descricao = mov.Descricao,
                    Valor = mov.Valor,
                    Tipo = mov.Tipo,
                    Categoria = mov.Categoria,
                    DataMovimento = mov.DataMovimento,
                    Status = mov.Status
                };
                
                await AbrirModalMovimentacao(clone);
            }
        }

        private async Task DeletarMovimentacaoAsync(object parametro)
        {
            if (parametro is MovimentacaoCaixa mov)
            {
                var resultado = System.Windows.MessageBox.Show(
                    $"Tem certeza que deseja excluir o lançamento '{mov.Descricao}' de {mov.Valor:C}?", 
                    "Confirmar Exclusão", 
                    System.Windows.MessageBoxButton.YesNo, 
                    System.Windows.MessageBoxImage.Warning);

                if (resultado == System.Windows.MessageBoxResult.Yes)
                {
                    await _service.ExcluirAsync(mov.Id);
                    await CarregarMovimentacoesAsync();
                    OnMovimentacaoSaved?.Invoke(); 
                }
            }
        }

        private async Task ReativarMovimentacaoAsync(object parametro)
        {
            if (parametro is MovimentacaoCaixa mov)
            {
                var resultado = System.Windows.MessageBox.Show(
                    $"Deseja reativar o lançamento '{mov.Descricao}' de {mov.Valor:C}?", 
                    "Confirmar Reativação", 
                    System.Windows.MessageBoxButton.YesNo, 
                    System.Windows.MessageBoxImage.Question);

                if (resultado == System.Windows.MessageBoxResult.Yes)
                {
                    await _service.ReativarAsync(mov.Id);
                    await CarregarMovimentacoesAsync();
                    OnMovimentacaoSaved?.Invoke(); 
                }
            }
        }


        private async Task CarregarMovimentacoesAsync()
        {
            try
            {
                string categoriaFiltro = _categoriaSelecionada == "Todas" ? null : _categoriaSelecionada;
                TipoMovimentacao? tipoFiltro = _tipoSelecionado switch
                {
                    "Entrada" => TipoMovimentacao.Entrada,
                    "Saída" => TipoMovimentacao.Saida,
                    _ => null
                };

                bool ativoFiltro = !SomenteInativos;

                var lista = await _service.ListarPorFiltrosAsync(TextoBusca, DataInicio, DataFim, categoriaFiltro, tipoFiltro, ativoFiltro);
                Movimentacoes = new ObservableCollection<MovimentacaoCaixa>(lista);
                MensagemErroFiltro = string.Empty; 
            }
            catch (System.ArgumentException ex)
            {
                MensagemErroFiltro = ex.Message;
                Movimentacoes = new ObservableCollection<MovimentacaoCaixa>();
            }
        }
    }
}

