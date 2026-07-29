using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using ControleCaixa.Model;
using ControleCaixa.Model.Enums;
using ControleCaixa.Model.Interfaces;
using ControleCaixa.Business.Services;
using MaterialDesignThemes.Wpf;

namespace PDVnet.ControleCaixa.UI.ViewModels
{
    public class MovimentacaoFormViewModel : BaseViewModel, IDataErrorInfo
    {
        private readonly ICategoriaService _categoriaService;
        private MovimentacaoCaixa _movimentacao;
        
        public bool IsEditMode => _movimentacao.Id > 0;
        public string Titulo => IsEditMode ? "Editar Movimentação" : "Nova Movimentação";

        public List<TipoMovimentacao> TiposDisponiveis { get; } = new List<TipoMovimentacao> 
        { 
            TipoMovimentacao.Entrada, 
            TipoMovimentacao.Saida 
        };

        private ObservableCollection<Categoria> _categoriasDisponiveis;
        public ObservableCollection<Categoria> CategoriasDisponiveis
        {
            get => _categoriasDisponiveis;
            private set => SetProperty(ref _categoriasDisponiveis, value);
        }

        private string _descricao;
        public string Descricao
        {
            get => _descricao;
            set => SetProperty(ref _descricao, value);
        }

        private decimal _valor;
        public decimal Valor
        {
            get => _valor;
            private set => SetProperty(ref _valor, value);
        }

        private string _valorTexto;
        public string ValorTexto
        {
            get => _valorTexto;
            set
            {
                if (SetProperty(ref _valorTexto, value))
                {
                    var normalizado = (value ?? "").Replace(',', '.');
                    if (decimal.TryParse(normalizado, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal resultado))
                        _valor = resultado;
                    else
                        _valor = 0;

                    OnPropertyChanged(nameof(Valor));
                }
            }
        }

        private TipoMovimentacao _tipo;
        public TipoMovimentacao Tipo
        {
            get => _tipo;
            set => SetProperty(ref _tipo, value);
        }

        private int? _categoriaId;
        public int? CategoriaId
        {
            get => _categoriaId;
            set => SetProperty(ref _categoriaId, value);
        }

        public ICommand SalvarCommand { get; }
        public ICommand CancelarCommand { get; }

        public MovimentacaoFormViewModel(ICategoriaService categoriaService, MovimentacaoCaixa movimentacaoExistente = null)
        {
            _categoriaService = categoriaService;
            CategoriasDisponiveis = new ObservableCollection<Categoria>();

            _movimentacao = movimentacaoExistente ?? new MovimentacaoCaixa() 
            { 
                DataMovimento = DateTime.Now,
                Descricao = ""
            };

            Descricao = _movimentacao.Descricao ?? "";
            ValorTexto = _movimentacao.Valor > 0 ? _movimentacao.Valor.ToString("F2", CultureInfo.InvariantCulture) : "";
            Tipo = _movimentacao.Tipo;
            CategoriaId = _movimentacao.CategoriaId;

            SalvarCommand = new RelayCommand(_ => Salvar(), _ => PodeSalvar());
            
            CancelarCommand = new RelayCommand(_ => DialogHost.CloseDialogCommand.Execute(false, null));

            _ = CarregarCategoriasAsync();
        }

        private async Task CarregarCategoriasAsync()
        {
            var cats = await _categoriaService.ListarTodasAsync();
            CategoriasDisponiveis = new ObservableCollection<Categoria>(cats);

            if (_movimentacao.Id == 0 && !CategoriaId.HasValue && CategoriasDisponiveis.Count > 0)
            {
                CategoriaId = CategoriasDisponiveis[0].Id;
            }
        }

        private void Salvar()
        {
            _movimentacao.Descricao = Descricao;
            _movimentacao.Valor = Valor;
            _movimentacao.Tipo = Tipo;
            _movimentacao.CategoriaId = CategoriaId;

            DialogHost.CloseDialogCommand.Execute(_movimentacao, null);
        }

        private bool PodeSalvar()
        {
            return string.IsNullOrEmpty(Error);
        }

        #region IDataErrorInfo (Validações da Tela)
        
        public string Error
        {
            get
            {
                var errors = new[] { this[nameof(Descricao)], this[nameof(ValorTexto)], this[nameof(CategoriaId)], this[nameof(Tipo)] };
                return errors.Any(e => e != null) ? "Erros no formulário" : null;
            }
        }

        public string this[string columnName]
        {
            get
            {
                string result = null;
                
                if (columnName == nameof(Descricao) && string.IsNullOrWhiteSpace(Descricao))
                    result = "A descrição é obrigatória.";
                
                if (columnName == nameof(ValorTexto) && Valor <= 0)
                    result = "O valor deve ser maior que zero.";
                
                if (columnName == nameof(CategoriaId) && !CategoriaId.HasValue)
                    result = "A categoria é obrigatória.";

                if (columnName == nameof(Tipo) && (!Enum.IsDefined(typeof(TipoMovimentacao), Tipo) || (int)Tipo == 0))
                    result = "O tipo da movimentação é obrigatório.";

                CommandManager.InvalidateRequerySuggested();
                
                return result;
            }
        }

        #endregion
    }
}
