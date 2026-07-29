using System.Collections.Generic;
using System.Threading.Tasks;

namespace ControleCaixa.Model.Interfaces
{
    public interface ICategoriaRepository
    {
        Task<List<Categoria>> ListarTodasAsync();
        Task AdicionarAsync(Categoria categoria);
        Task DeletarAsync(int id);
        Task<bool> EmUsoAsync(int id);
    }
}
