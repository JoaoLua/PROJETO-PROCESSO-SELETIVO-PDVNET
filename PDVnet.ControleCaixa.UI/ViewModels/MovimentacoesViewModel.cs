using System.Collections.ObjectModel;
using System.Windows.Input;
using ControleCaixa.Model;
using ControleCaixa.Bussines.Services;
using ControleCaixa.Data;
using MaterialDesignThemes.Wpf;
using PDVnet.ControleCaixa.UI.Views;
using System.Threading.Tasks;

namespace PDVnet.ControleCaixa.UI.ViewModels
{
    public class MovimentacoesViewModel : BaseViewModel
    {
        private readonly MovimentacaoService _service;
        
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
                    CarregarMovimentacoes();
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
                    CarregarMovimentacoes();
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
                    CarregarMovimentacoes();
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

        public ICommand BuscarCommand { get; }
        public ICommand EditarCommand { get; }
        public ICommand DeletarCommand { get; }
        public ICommand NovaMovimentacaoCommand { get; }

        private readonly Action _onSavedCallback;

        public MovimentacoesViewModel(Action onSavedCallback = null)
        {
            _onSavedCallback = onSavedCallback;
            
            // Instanciando o serviço manualmente (o ideal futuro é usar Injeção de Dependência)
            _service = new MovimentacaoService(new MovimentacaoRepository());
            
            BuscarCommand = new RelayCommand(_ => CarregarMovimentacoes());
            
            // Usando async void para os comandos abrirem o modal de forma assíncrona
            NovaMovimentacaoCommand = new RelayCommand(async _ => await AbrirModalMovimentacao());
            EditarCommand = new RelayCommand(async parametro => await EditarMovimentacao(parametro));
            
            DeletarCommand = new RelayCommand(DeletarMovimentacao);
            
            CarregarMovimentacoes();
        }

        private async Task AbrirModalMovimentacao(MovimentacaoCaixa movimentacaoExistente = null)
        {
            var formVm = new MovimentacaoFormViewModel(movimentacaoExistente);
            var formView = new MovimentacaoFormView { DataContext = formVm };

            // "RootDialog" é o nome do container de Dialog que definimos na MainWindow.xaml
            var result = await DialogHost.Show(formView, "RootDialog");

            // Se o resultado for uma MovimentacaoCaixa, significa que o usuário clicou em "Salvar"
            if (result is MovimentacaoCaixa mov)
            {
                if (mov.Id == 0)
                {
                    _service.Inserir(mov);
                }
                else
                {
                    _service.Atualizar(mov);
                }
                CarregarMovimentacoes(); // Recarrega a tabela com os novos dados
                _onSavedCallback?.Invoke(); // Avisa a MainViewModel para atualizar o saldo
            }
        }

        private async Task EditarMovimentacao(object parametro)
        {
            if (parametro is MovimentacaoCaixa mov)
            {
                // Criamos um clone para que, se o usuário cancelar, a grid não seja alterada incorretamente
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

        private void DeletarMovimentacao(object parametro)
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
                    _service.Excluir(mov.Id);
                    CarregarMovimentacoes(); // Recarrega a tabela após excluir
                    _onSavedCallback?.Invoke(); // Avisa a MainViewModel para atualizar o saldo
                }
            }
        }

        private void CarregarMovimentacoes()
        {
            try
            {
                var lista = _service.ListarPorFiltros(TextoBusca, DataInicio, DataFim);
                Movimentacoes = new ObservableCollection<MovimentacaoCaixa>(lista);
                MensagemErroFiltro = string.Empty; // Limpa a mensagem se deu certo
            }
            catch (System.ArgumentException ex)
            {
                MensagemErroFiltro = ex.Message;
                // Opcional: pode esvaziar a lista quando o filtro está inválido
                Movimentacoes = new ObservableCollection<MovimentacaoCaixa>();
            }
        }
    }
}
