using System.Collections.Generic;
using System.Threading.Tasks;

namespace ControleCaixa.Model.Interfaces
{
    public interface ICategoriaService
    {
        Task<List<Categoria>> ListarTodasAsync();
        Task AdicionarAsync(Categoria categoria);
        Task DeletarAsync(int id);
    }
}
