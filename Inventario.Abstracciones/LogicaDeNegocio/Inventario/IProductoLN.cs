using Inventario.Abstracciones.ModelosParaUI;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Inventario.Abstracciones.LogicaDeNegocio.Producto
{
    public interface IProductoLN
    {
        Task<int> Guardar(ProductoDto producto);
        Task<int> Actualizar(ProductoDto producto);
        Task<int> Eliminar(int id);
        ProductoDto ObtenerPorId(int id);
        List<ProductoDto> ObtenerTodos();
    }
}