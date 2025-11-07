using Inventario.Abstracciones.LogicaDeNegocio.Producto;
using Inventario.Abstracciones.ModelosParaUI;
using Inventario.AccesoADatos.Producto; 
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Inventario.LogicaDeNegocio.Producto
{
    public class ProductoLN : IProductoLN
    {
        private readonly ProductoRepository _productoRepository;

        public ProductoLN()
        {
            _productoRepository = new ProductoRepository();
        }

        public async Task<int> Actualizar(ProductoDto producto)
        {
            return await Task.FromResult(_productoRepository.Actualizar(producto));
        }

        public async Task<int> Eliminar(int id)
        {
            return await Task.FromResult(_productoRepository.Eliminar(id));
        }

        public async Task<int> Guardar(ProductoDto producto)
        {
            return await Task.FromResult(_productoRepository.Guardar(producto));
        }

        public ProductoDto ObtenerPorId(int id)
        {
            return _productoRepository.ObtenerPorId(id);
        }

        public List<ProductoDto> ObtenerTodos()
        {
            return _productoRepository.ObtenerTodos();
        }
    }
}