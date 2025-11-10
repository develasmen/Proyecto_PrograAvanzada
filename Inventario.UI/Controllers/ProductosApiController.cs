using Inventario.Abstracciones.ModelosParaUI;
using Inventario.LogicaDeNegocio.Producto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Inventario.UI.Controllers
{
    [Authorize]
    [RoutePrefix("api/productos")]
    public class ProductosApiController : ApiController
    {
        private readonly ProductoLN _productoLN;

        public ProductosApiController()
        {
            _productoLN = new ProductoLN();
        }

        [HttpGet]
        [Route("buscar")]
        public IHttpActionResult Buscar(string termino = "", bool soloActivos = false, bool soloConStock = false)
        {
            var productos = _productoLN.ObtenerTodos();

            if (!string.IsNullOrWhiteSpace(termino))
            {
                termino = termino.Trim();
                productos = FiltrarPorTermino(productos, termino);
            }

            if (soloActivos)
            {
                productos = productos.Where(p => p.Estado).ToList();
            }

            if (soloConStock)
            {
                productos = productos.Where(p => p.CantidadEnStock > 0).ToList();
            }

            return Ok(productos);
        }

        private static List<ProductoDto> FiltrarPorTermino(IEnumerable<ProductoDto> productos, string termino)
        {
            var terminoComparacion = termino.ToLowerInvariant();

            return productos.Where(p =>
                (!string.IsNullOrWhiteSpace(p.Nombre) && p.Nombre.ToLowerInvariant().Contains(terminoComparacion)) ||
                (!string.IsNullOrWhiteSpace(p.Marca) && p.Marca.ToLowerInvariant().Contains(terminoComparacion)) ||
                (!string.IsNullOrWhiteSpace(p.SKU) && p.SKU.ToLowerInvariant().Contains(terminoComparacion)))
                .ToList();
        }
    }
}
