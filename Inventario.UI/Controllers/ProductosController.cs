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
        public ActionResult ImagenDeProductoPorSKU(string sku)
        {
            if (string.IsNullOrWhiteSpace(sku))
            {
                return PlaceholderImage();
            }

            string carpeta = Server.MapPath("~/Content/Upload");
            string[] extensiones = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".svg" };
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
                case ".jpeg": return "image/jpeg";
                case ".png": return "image/png";
                case ".gif": return "image/gif";
                case ".webp": return "image/webp";
                case ".svg": return "image/svg+xml";
                default: return "application/octet-stream";
            }
        }

        private void GuardarArchivo(HttpPostedFileBase archivo, string nombreBase)
        {
            // 🔍 DEBUG
            System.Diagnostics.Debug.WriteLine("═══════════════════════════════");
            System.Diagnostics.Debug.WriteLine("🔍 MÉTODO GuardarArchivo LLAMADO");
            System.Diagnostics.Debug.WriteLine($"📄 Archivo es null?: {archivo == null}");

            if (archivo != null)
            {
                System.Diagnostics.Debug.WriteLine($"📄 ContentLength: {archivo.ContentLength}");
                System.Diagnostics.Debug.WriteLine($"📄 FileName: {archivo.FileName}");
            }

            System.Diagnostics.Debug.WriteLine($"📝 nombreBase (SKU): {nombreBase}");

            if (archivo == null || archivo.ContentLength <= 0 || string.IsNullOrWhiteSpace(nombreBase))
            {
                System.Diagnostics.Debug.WriteLine("❌ SALIENDO: archivo nulo, vacío o sin nombreBase");
                System.Diagnostics.Debug.WriteLine("═══════════════════════════════");
                return;
            }

            string carpeta = Server.MapPath("~/Content/Upload");
            System.Diagnostics.Debug.WriteLine($"📂 Carpeta destino: {carpeta}");
            System.Diagnostics.Debug.WriteLine($"📂 Carpeta existe?: {Directory.Exists(carpeta)}");

            if (!Directory.Exists(carpeta))
            {
                Directory.CreateDirectory(carpeta);
                System.Diagnostics.Debug.WriteLine("✅ Carpeta creada");
            }

            string extension = Path.GetExtension(archivo.FileName);
            if (string.IsNullOrEmpty(extension)) extension = ".png";
            string rutaDestino = Path.Combine(carpeta, nombreBase + extension.ToLowerInvariant());

            System.Diagnostics.Debug.WriteLine($"💾 Ruta destino completa: {rutaDestino}");

            // Borrar archivos anteriores con el mismo nombreBase
            foreach (var existente in Directory.GetFiles(carpeta, nombreBase + ".*"))
            {
                try
                {
                    System.IO.File.Delete(existente);
                    System.Diagnostics.Debug.WriteLine($"🗑️ Eliminado: {existente}");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"⚠️ Error al eliminar: {ex.Message}");
                }
            }

            try
            {
                archivo.SaveAs(rutaDestino);
                System.Diagnostics.Debug.WriteLine("✅✅✅ ARCHIVO GUARDADO EXITOSAMENTE");
                System.Diagnostics.Debug.WriteLine($"✅ Verificando: Archivo existe? {System.IO.File.Exists(rutaDestino)}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌❌❌ ERROR AL GUARDAR: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");
            }

            System.Diagnostics.Debug.WriteLine("═══════════════════════════════");
        }
    }
}