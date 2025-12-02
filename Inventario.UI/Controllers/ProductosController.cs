using Inventario.Abstracciones.ModelosParaUI;
using Inventario.LogicaDeNegocio.Producto;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Inventario.UI.Controllers
{
    [Authorize(Roles = "Administrador")]
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
                if (productoCreado.archivo != null && productoCreado.archivo.ContentLength > 0)
                {
                    GuardarArchivo(productoCreado.archivo, productoCreado.SKU);
                }

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

                if (productoEditado.archivo != null && productoEditado.archivo.ContentLength > 0)
                {
                    GuardarArchivo(productoEditado.archivo, productoEditado.SKU);
                }

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

        // Devuelve la imagen del producto por SKU o un placeholder si no existe
        // Permiso anónimo para poder ver la imagen aunque no sea Admin para el catálogo
        [AllowAnonymous]
        public ActionResult ImagenDeProductoPorSKU(string sku)
        {
            if (string.IsNullOrWhiteSpace(sku))
            {
                return PlaceholderImage();
            }

            string carpeta = Server.MapPath("~/Content/Uploads");
<<<<<<< HEAD
            string[] extensiones = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".svg", ".jfif" };
=======
            string[] extensiones = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".svg" };
>>>>>>> parent of a834f71 (Inicialializacion de los placeholders por imagenes en la bd)
            foreach (var ext in extensiones)
            {
                string ruta = Path.Combine(carpeta, sku + ext);
                if (System.IO.File.Exists(ruta))
                {
                    string contentType = ObtenerContentType(ext);
                    return File(ruta, contentType);
                }
            }
            return PlaceholderImage();
        }
        //Permiso anónimo para poder ver la imagen aunque no sea Admin para el catálogo
        [AllowAnonymous]
        private ActionResult PlaceholderImage()
        {
            string placeholder = Server.MapPath("~/Content/Images/placeholder.svg");
            if (System.IO.File.Exists(placeholder))
            {
                return File(placeholder, "image/svg+xml");
            }
            return new HttpNotFoundResult();
        }

        private string ObtenerContentType(string ext)
        {
            switch (ext.ToLowerInvariant())
            {
                case ".jpg":
                case ".jpeg":
                case ".jfif": return "image/jpeg";
                case ".png": return "image/png";
                case ".gif": return "image/gif";
                case ".webp": return "image/webp";
                case ".svg": return "image/svg+xml";
                default: return "application/octet-stream";
            }
        }

        private void GuardarArchivo(HttpPostedFileBase archivo, string nombreBase)
        {
            if (archivo == null || archivo.ContentLength <= 0 || string.IsNullOrWhiteSpace(nombreBase))
                return;

            string carpeta = Server.MapPath("~/Content/Uploads");
            if (!Directory.Exists(carpeta)) Directory.CreateDirectory(carpeta);

            string extension = Path.GetExtension(archivo.FileName);
            if (string.IsNullOrEmpty(extension)) extension = ".png";
            string rutaDestino = Path.Combine(carpeta, nombreBase + extension.ToLowerInvariant());

            foreach (var existente in Directory.GetFiles(carpeta, nombreBase + ".*"))
            {
                try { System.IO.File.Delete(existente); } catch { }
            }

            archivo.SaveAs(rutaDestino);
        }
    }
}