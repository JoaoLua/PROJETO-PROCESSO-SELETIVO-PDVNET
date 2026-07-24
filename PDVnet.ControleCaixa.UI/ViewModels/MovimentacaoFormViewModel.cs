using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows.Input;
using ControleCaixa.Model;
using ControleCaixa.Model.Enums;
using MaterialDesignThemes.Wpf;

namespace PDVnet.ControleCaixa.UI.ViewModels
{
    public class MovimentacaoFormViewModel : BaseViewModel, IDataErrorInfo
    {
        private MovimentacaoCaixa _movimentacao;
        
        public bool IsEditMode => _movimentacao.Id > 0;
        public string Titulo => IsEditMode ? "Editar Movimentação" : "Nova Movimentação";

        public List<TipoMovimentacao> TiposDisponiveis { get; } = new List<TipoMovimentacao> 
        { 
            TipoMovimentacao.Entrada, 
            TipoMovimentacao.Saida 
        };

        public List<string> CategoriasDisponiveis { get; } = new List<string>
        {
            "Vendas",
            "Pagamentos",
            "Serviços Agregados",
            "Recebimento de Fornecedores",
            "Contas de Consumo",
            "Salário",
            "Manutenção e Reparos",
            "Outros"
        };

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
                    // Normaliza: troca vírgula por ponto para parsing
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

        private string _categoria;
        public string Categoria
        {
            get => _categoria;
            set => SetProperty(ref _categoria, value);
        }

        public ICommand SalvarCommand { get; }
        public ICommand CancelarCommand { get; }

        public MovimentacaoFormViewModel(MovimentacaoCaixa movimentacaoExistente = null)
        {

            _movimentacao = movimentacaoExistente ?? new MovimentacaoCaixa() 
            { 
                DataMovimento = DateTime.Now,
                Descricao = "",
                Categoria = ""
            };

            Descricao = _movimentacao.Descricao ?? "";
            ValorTexto = _movimentacao.Valor > 0 ? _movimentacao.Valor.ToString("F2", CultureInfo.InvariantCulture) : "";
            Tipo = _movimentacao.Tipo;
            Categoria = string.IsNullOrEmpty(_movimentacao.Categoria) ? CategoriasDisponiveis[0] : _movimentacao.Categoria;

            SalvarCommand = new RelayCommand(_ => Salvar(), _ => PodeSalvar());
            
            CancelarCommand = new RelayCommand(_ => DialogHost.CloseDialogCommand.Execute(false, null));
        }

        private void Salvar()
        {
            _movimentacao.Descricao = Descricao;
            _movimentacao.Valor = Valor;
            _movimentacao.Tipo = Tipo;
            _movimentacao.Categoria = Categoria;

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
                var errors = new[] { this[nameof(Descricao)], this[nameof(ValorTexto)], this[nameof(Categoria)] };
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
                
                if (columnName == nameof(Categoria) && string.IsNullOrWhiteSpace(Categoria))
                    result = "A categoria é obrigatória.";

                CommandManager.InvalidateRequerySuggested();
                
                return result;
            }
        }

        #endregion
    }
}
