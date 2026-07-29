using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ControleCaixa.Model;
using ControleCaixa.Model.Interfaces;

namespace ControleCaixa.Business.Services
{
    public class CategoriaService : ICategoriaService
    {
        private readonly ICategoriaRepository _repository;

        public CategoriaService(ICategoriaRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<Categoria>> ListarTodasAsync()
        {
            return await _repository.ListarTodasAsync();
        }

        public async Task AdicionarAsync(Categoria categoria)
        {
            if (string.IsNullOrWhiteSpace(categoria.Nome))
                throw new ArgumentException("O nome da categoria é obrigatório.");
            if (categoria.Nome.Length > 100)
                throw new ArgumentException("O nome da categoria deve ter no máximo 100 caracteres.");

            await _repository.AdicionarAsync(categoria);
        }

        public async Task DeletarAsync(int id)
        {
            if (await _repository.EmUsoAsync(id))
            {
                throw new InvalidOperationException("Não é possível excluir esta categoria pois ela já está vinculada a movimentações no sistema.");
            }
            
            await _repository.DeletarAsync(id);
        }
    }
}
