using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using System.Threading.Tasks;
using ControleCaixa.Business.Services;
using ControleCaixa.Model;
using ControleCaixa.Model.Interfaces;

namespace PDVnet.ControleCaixa.UI.ViewModels
{
    public class CategoriasViewModel : BaseViewModel
    {
        private readonly ICategoriaService _service;

        private ObservableCollection<Categoria> _categorias;
        public ObservableCollection<Categoria> Categorias
        {
            get => _categorias;
            set => SetProperty(ref _categorias, value);
        }

        private string _novaCategoriaNome;
        public string NovaCategoriaNome
        {
            get => _novaCategoriaNome;
            set
            {
                SetProperty(ref _novaCategoriaNome, value);
                CommandManager.InvalidateRequerySuggested();
            }
        }

        private string _mensagemErro;
        public string MensagemErro
        {
            get => _mensagemErro;
            set => SetProperty(ref _mensagemErro, value);
        }

        public ICommand AdicionarCommand { get; }
        public ICommand RemoverCommand { get; }

        public CategoriasViewModel(ICategoriaService service)
        {
            _service = service;
            AdicionarCommand = new RelayCommand(async _ => await AdicionarAsync(), _ => !string.IsNullOrWhiteSpace(NovaCategoriaNome));
            RemoverCommand = new RelayCommand(async id => await RemoverAsync((int)id));

            _ = CarregarCategoriasAsync();
        }

        private async Task CarregarCategoriasAsync()
        {
            try
            {
                var lista = await _service.ListarTodasAsync();
                Categorias = new ObservableCollection<Categoria>(lista);
            }
            catch (Exception ex)
            {
                MensagemErro = "Erro ao carregar categorias: " + ex.Message;
            }
        }

        private async Task AdicionarAsync()
        {
            try
            {
                MensagemErro = string.Empty;
                var categoria = new Categoria { Nome = NovaCategoriaNome.Trim() };
                await _service.AdicionarAsync(categoria);
                NovaCategoriaNome = string.Empty;
                await CarregarCategoriasAsync();
            }
            catch (Exception ex)
            {
                MensagemErro = ex.Message;
            }
        }

        private async Task RemoverAsync(int id)
        {
            try
            {
                MensagemErro = string.Empty;
                await _service.DeletarAsync(id);
                await CarregarCategoriasAsync();
            }
            catch (Exception ex)
            {
                MensagemErro = ex.Message;
            }
        }
    }
}
