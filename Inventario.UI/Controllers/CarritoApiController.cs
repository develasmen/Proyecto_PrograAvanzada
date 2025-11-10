using Inventario.Abstracciones.ModelosParaUI;
using Inventario.LogicaDeNegocio.Carrito;
using Microsoft.AspNet.Identity;
using System;
using System.Threading.Tasks;
using System.Web.Http;

namespace Inventario.UI.Controllers
{
    [System.Web.Http.Authorize]
    [System.Web.Http.RoutePrefix("api/carrito")]
    public class CarritoApiController : ApiController
    {
        private readonly CarritoLN _carritoLN;

        public CarritoApiController()
        {
            _carritoLN = new CarritoLN();
        }

        // GET: api/carrito
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("")]
        public async Task<IHttpActionResult> ObtenerCarrito()
        {
            try
            {
                var usuarioId = User.Identity.GetUserId();
                var items = await _carritoLN.ObtenerCarritoPorUsuario(usuarioId);
                return Ok(items);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: api/carrito/resumen
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("resumen")]
        public async Task<IHttpActionResult> ObtenerResumen()
        {
            try
            {
                var usuarioId = User.Identity.GetUserId();
                var resumen = await _carritoLN.ObtenerResumen(usuarioId);
                return Ok(resumen);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST: api/carrito/agregar
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("agregar")]
        public async Task<IHttpActionResult> AgregarAlCarrito([FromBody] AgregarAlCarritoRequest request)
        {
            try
            {
                if (request == null || request.ProductoId <= 0 || request.Cantidad <= 0)
                    return BadRequest("Datos inválidos");

                var usuarioId = User.Identity.GetUserId();
                var item = await _carritoLN.AgregarAlCarrito(usuarioId, request.ProductoId, request.Cantidad);
                return Ok(item);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT: api/carrito/actualizar
        [System.Web.Http.HttpPut]
        [System.Web.Http.Route("actualizar")]
        public async Task<IHttpActionResult> ActualizarCantidad([FromBody] ActualizarCarritoRequest request)
        {
            try
            {
                if (request == null || request.CarritoId <= 0 || request.Cantidad <= 0)
                    return BadRequest("Datos inválidos");

                var item = await _carritoLN.ActualizarCantidad(request.CarritoId, request.Cantidad);
                return Ok(item);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE: api/carrito/eliminar/5
        [System.Web.Http.HttpDelete]
        [System.Web.Http.Route("eliminar/{id}")]
        public async Task<IHttpActionResult> EliminarDelCarrito(int id)
        {
            try
            {
                var usuarioId = User.Identity.GetUserId();
                var resultado = await _carritoLN.EliminarDelCarrito(id, usuarioId);

                if (!resultado)
                    return NotFound();

                return Ok(new { mensaje = "Producto eliminado del carrito" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE: api/carrito/vaciar
        [System.Web.Http.HttpDelete]
        [System.Web.Http.Route("vaciar")]
        public async Task<IHttpActionResult> VaciarCarrito()
        {
            try
            {
                var usuarioId = User.Identity.GetUserId();
                await _carritoLN.VaciarCarrito(usuarioId);
                return Ok(new { mensaje = "Carrito vaciado exitosamente" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST: api/carrito/finalizar-compra
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("finalizar-compra")]
        public async Task<IHttpActionResult> FinalizarCompra()
        {
            try
            {
                var usuarioId = User.Identity.GetUserId();
                var resultado = await _carritoLN.FinalizarCompra(usuarioId);

                if (!resultado)
                    return BadRequest("No se pudo procesar la compra");

                return Ok(new { mensaje = "Compra finalizada exitosamente" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}