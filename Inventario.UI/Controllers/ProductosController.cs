using Inventario.Abstracciones.ModelosParaUI;
using Inventario.LogicaDeNegocio.Producto; 
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Inventario.UI.Controllers
{
    [Authorize(Roles = "Administrador")] //Control de usuarios (solo lo puede ver el admin)
    public class ProductosController : Controller
    {
        private readonly ProductoLN _productoLN;

        public ProductosController()
        {
            _productoLN = new ProductoLN();
        }

        // GET: Productos
        public ActionResult ListarProductos()
        {
            List<ProductoDto> listaDeProductos = _productoLN.ObtenerTodos();
            return View(listaDeProductos);
        }

        // GET: Productos/DetallesProducto/5
        public ActionResult DetallesProducto(int id)
        {
            ProductoDto producto = _productoLN.ObtenerPorId(id);
            if (producto == null)
            {
                return HttpNotFound();
            }
            return View(producto);
        }

        // GET: Productos/CrearProducto
        public ActionResult CrearProducto()
        {
            return View("CrearOEditarProducto", new ProductoDto());
        }

        // POST: Productos/CrearProducto
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CrearProducto(ProductoDto productoCreado)
        {
            if (ModelState.IsValid)
            {
                await _productoLN.Guardar(productoCreado);
                return RedirectToAction("ListarProductos");
            }
            return View("CrearOEditarProducto", productoCreado);
        }

        // GET: Productos/EditarProducto/5
        public ActionResult EditarProducto(int id)
        {
            ProductoDto producto = _productoLN.ObtenerPorId(id);
            if (producto == null)
            {
                return HttpNotFound();
            }
            return View("CrearOEditarProducto", producto);
        }

        // POST: Productos/EditarProducto/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditarProducto(int id, ProductoDto productoEditado)
        {
            if (ModelState.IsValid)
            {
                productoEditado.Id = id;
                await _productoLN.Actualizar(productoEditado);
                return RedirectToAction("ListarProductos");
            }
            return View("CrearOEditarProducto", productoEditado);
        }

        // GET: Productos/EliminarProducto/5
        public ActionResult EliminarProducto(int id)
        {
            ProductoDto producto = _productoLN.ObtenerPorId(id);
            if (producto == null)
            {
                return HttpNotFound();
            }
            return View(producto);
        }

        // POST: Productos/EliminarProducto/5
        [HttpPost, ActionName("EliminarProducto")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EliminarConfirmado(int id)
        {
            await _productoLN.Eliminar(id);
            return RedirectToAction("ListarProductos");
        }
    }
}