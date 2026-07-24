using System;
using System.Collections.Generic;
using System.ComponentModel;
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
            "Alimentação",
            "Transporte",
            "Vendas",
            "Pagamentos",
            "Salário",
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
            set => SetProperty(ref _valor, value);
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
            // Se vier nulo, cria uma nova inicializando as propriedades required.
            _movimentacao = movimentacaoExistente ?? new MovimentacaoCaixa() 
            { 
                DataMovimento = DateTime.Now,
                Descricao = "",
                Categoria = ""
            };

            Descricao = _movimentacao.Descricao ?? "";
            Valor = _movimentacao.Valor;
            Tipo = _movimentacao.Tipo;
            Categoria = string.IsNullOrEmpty(_movimentacao.Categoria) ? CategoriasDisponiveis[0] : _movimentacao.Categoria;

            SalvarCommand = new RelayCommand(_ => Salvar(), _ => PodeSalvar());
            
            // O comando de fechar do MaterialDesign recebe o valor de retorno (false significa cancelar)
            CancelarCommand = new RelayCommand(_ => DialogHost.CloseDialogCommand.Execute(false, null));
        }

        private void Salvar()
        {
            _movimentacao.Descricao = Descricao;
            _movimentacao.Valor = Valor;
            _movimentacao.Tipo = Tipo;
            _movimentacao.Categoria = Categoria;

            // Retorna a movimentação preenchida para quem abriu o Modal
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
                var errors = new[] { this[nameof(Descricao)], this[nameof(Valor)], this[nameof(Categoria)] };
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
                
                // Validação para não aceitar valores negativos ou zero
                if (columnName == nameof(Valor) && Valor <= 0)
                    result = "O valor deve ser maior que zero.";
                
                if (columnName == nameof(Categoria) && string.IsNullOrWhiteSpace(Categoria))
                    result = "A categoria é obrigatória.";

                // Atualiza o estado do botão Salvar toda vez que uma validação é checada
                CommandManager.InvalidateRequerySuggested();
                
                return result;
            }
        }

        #endregion
    }
}
