using System;
using System.Threading.Tasks;
using System.Web.Http;
using Inventario.Abstracciones.ModelosParaUI;
using Inventario.LogicaDeNegocio.Carrito;
using Microsoft.AspNet.Identity;

namespace Inventario.UI.Controllers
{
    [Authorize]
    [RoutePrefix("api/carrito")]
    public class CarritoApiController : ApiController
    {
        private readonly CarritoLN _carritoLN;

        public CarritoApiController()
        {
            _carritoLN = new CarritoLN();
        }

        [HttpGet]
        [Route("")]
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

        [HttpGet]
        [Route("resumen")]
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

        [HttpPost]
        [Route("agregar")]
        public async Task<IHttpActionResult> AgregarAlCarrito([FromBody] AgregarAlCarritoRequest request)
        {
            try
            {
                if (request == null || request.ProductoId <= 0 || request.Cantidad <= 0)
                    return BadRequest("Datos inválidos");

                var usuarioId = User.Identity.GetUserId();
                var item = await _carritoLN.AgregarAlCarrito(usuarioId, request.ProductoId, request.Cantidad);
                var resumen = await _carritoLN.ObtenerResumen(usuarioId);
                return Ok(new { item, resumen });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        [Route("actualizar")]
        public async Task<IHttpActionResult> ActualizarCantidad([FromBody] ActualizarCarritoRequest request)
        {
            try
            {
                if (request == null || request.CarritoId <= 0 || request.Cantidad <= 0)
                    return BadRequest("Datos inválidos");

                var usuarioId = User.Identity.GetUserId();
                var item = await _carritoLN.ActualizarCantidad(request.CarritoId, request.Cantidad);
                var resumen = await _carritoLN.ObtenerResumen(usuarioId);
                return Ok(new { item, resumen });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        [Route("eliminar/{id}")]
        public async Task<IHttpActionResult> EliminarDelCarrito(int id)
        {
            try
            {
                var usuarioId = User.Identity.GetUserId();
                var resultado = await _carritoLN.EliminarDelCarrito(id, usuarioId);

                if (!resultado)
                    return NotFound();

                var resumen = await _carritoLN.ObtenerResumen(usuarioId);
                return Ok(new { mensaje = "Producto eliminado del carrito", resumen });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        [Route("vaciar")]
        public async Task<IHttpActionResult> VaciarCarrito()
        {
            try
            {
                var usuarioId = User.Identity.GetUserId();
                await _carritoLN.VaciarCarrito(usuarioId);
                var resumen = await _carritoLN.ObtenerResumen(usuarioId);
                return Ok(new { mensaje = "Carrito vaciado exitosamente", resumen });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("finalizar-compra")]
        public async Task<IHttpActionResult> FinalizarCompra()
        {
            try
            {
                var usuarioId = User.Identity.GetUserId();
                var resultado = await _carritoLN.FinalizarCompra(usuarioId);

                if (!resultado)
                    return BadRequest("No se pudo procesar la compra");

                var resumen = await _carritoLN.ObtenerResumen(usuarioId);
                return Ok(new { mensaje = "Compra finalizada exitosamente", resumen });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("aplicar-cupon")]
        public async Task<IHttpActionResult> AplicarCupon([FromBody] AplicarCuponRequest request)
        {
            try
            {
                var usuarioId = User.Identity.GetUserId();
                var item = await _carritoLN.AplicarCupon(usuarioId, request);
                var resumen = await _carritoLN.ObtenerResumen(usuarioId);
                return Ok(new { item, resumen });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        [Route("remover-cupon/{carritoId}")]
        public async Task<IHttpActionResult> RemoverCupon(int carritoId)
        {
            try
            {
                var usuarioId = User.Identity.GetUserId();
                var resultado = await _carritoLN.RemoverCupon(carritoId, usuarioId);
                if (!resultado)
                {
                    return NotFound();
                }

                var resumen = await _carritoLN.ObtenerResumen(usuarioId);
                return Ok(new { mensaje = "Cupón removido", resumen });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}

