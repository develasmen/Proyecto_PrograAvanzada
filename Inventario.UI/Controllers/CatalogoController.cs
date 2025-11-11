using Inventario.Abstracciones.ModelosParaUI;
using Inventario.LogicaDeNegocio.Producto;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Inventario.UI.Controllers
{
    [Authorize(Roles = "Ventas,Operaciones")] //Cambiamos esto a 2 roles, ya NO se usa "Clientes"
                                              //Vamos a dejar Operaciones ya que no tenemos ninguna vista para esos 
    public class CatalogoController : Controller
    {
        private readonly ProductoLN _productoLN;

        public CatalogoController()
        {
            _productoLN = new ProductoLN();
        }

        // GET: Catalogo
        public ActionResult Index()
        {
            // Obtener solo productos activos y con stock
            List<ProductoDto> productos = _productoLN.ObtenerTodos()
                .Where(p => p.Estado && p.CantidadEnStock > 0)
                .ToList();

            return View(productos);
        }
    }
}