using System.Collections.Generic;
using System.Threading.Tasks;
using Inventario.Abstracciones.ModelosParaUI;

namespace Inventario.Abstracciones.LogicaDeNegocio.Cupon
{
    public interface ICuponDescuentoLN
    {
        Task<int> Crear(CuponDescuentoDto cupon);
        Task<int> Actualizar(CuponDescuentoDto cupon);
        Task<int> Eliminar(int id);
        Task<CuponDescuentoDto> ObtenerPorId(int id);
        Task<CuponDescuentoDto> ObtenerPorCodigo(string codigo);
        Task<List<CuponDescuentoDto>> ObtenerTodos();
        Task RegistrarUso(int cuponId);
    }
}

