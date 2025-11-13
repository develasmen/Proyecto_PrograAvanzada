using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Inventario.Abstracciones.ModelosParaUI;
using Inventario.LogicaDeNegocio.Cupon;
using Inventario.LogicaDeNegocio.Producto;

namespace Inventario.UI.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class CuponesController : Controller
    {
        private readonly CuponDescuentoLN _cuponLn;
        private readonly ProductoLN _productoLn;

        public CuponesController()
        {
            _cuponLn = new CuponDescuentoLN();
            _productoLn = new ProductoLN();
        }

        public async Task<ActionResult> Index()
        {
            var cupones = await _cuponLn.ObtenerTodos();
            return View(cupones.OrderByDescending(c => c.FechaInicio).ToList());
        }

        public async Task<ActionResult> Crear()
        {
            await CargarProductos();
            return View("CrearOEditar", new CuponDescuentoDto
            {
                FechaInicio = DateTime.Now.Date,
                FechaFin = DateTime.Now.Date.AddMonths(1),
                Activo = true
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Crear(CuponDescuentoDto cupon)
        {
            if (!ModelState.IsValid)
            {
                await CargarProductos();
                return View("CrearOEditar", cupon);
            }

            await _cuponLn.Crear(cupon);
            return RedirectToAction("Index");
        }

        public async Task<ActionResult> Editar(int id)
        {
            var cupon = await _cuponLn.ObtenerPorId(id);
            if (cupon == null)
            {
                return HttpNotFound();
            }

            await CargarProductos(cupon.ProductoId);
            return View("CrearOEditar", cupon);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Editar(int id, CuponDescuentoDto cupon)
        {
            if (!ModelState.IsValid)
            {
                await CargarProductos(cupon.ProductoId);
                return View("CrearOEditar", cupon);
            }

            cupon.Id = id;
            await _cuponLn.Actualizar(cupon);
            return RedirectToAction("Index");
        }

        public async Task<ActionResult> Eliminar(int id)
        {
            var cupon = await _cuponLn.ObtenerPorId(id);
            if (cupon == null)
            {
                return HttpNotFound();
            }

            return View(cupon);
        }

        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EliminarConfirmado(int id)
        {
            await _cuponLn.Eliminar(id);
            return RedirectToAction("Index");
        }

        private async Task CargarProductos(int? seleccionado = null)
        {
            var productos = _productoLn.ObtenerTodos()
                .Where(p => p.Estado)
                .OrderBy(p => p.Nombre)
                .Select(p => new SelectListItem
                {
                    Text = $"{p.Nombre} ({p.SKU})",
                    Value = p.Id.ToString(),
                    Selected = seleccionado.HasValue && seleccionado.Value == p.Id
                })
                .ToList();

            productos.Insert(0, new SelectListItem { Text = "Aplicable a cualquier producto", Value = string.Empty, Selected = !seleccionado.HasValue });
            ViewBag.Productos = productos;
        }
    }
}

