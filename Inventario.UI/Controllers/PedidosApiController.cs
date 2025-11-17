using Inventario.LogicaDeNegocio.Pedido;
using Microsoft.AspNet.Identity;
using System;
using System.Threading.Tasks;
using System.Web.Http;

namespace Inventario.UI.Controllers
{
    [Authorize]
    [RoutePrefix("api/pedidos")]
    public class PedidosApiController : ApiController
    {
        private readonly PedidoLN _pedidoLN;

        public PedidosApiController()
        {
            _pedidoLN = new PedidoLN();
        }

        // GET: api/pedidos
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("")]
        [Authorize(Roles = "Administrador")]
        public IHttpActionResult ObtenerTodos()
        {
            try
            {
                var pedidos = _pedidoLN.ObtenerTodos();
                return Ok(pedidos);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: api/pedidos/mis-pedidos
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("mis-pedidos")]
        public IHttpActionResult ObtenerMisPedidos()
        {
            try
            {
                var usuarioId = User.Identity.GetUserId();
                var pedidos = _pedidoLN.ObtenerPorUsuario(usuarioId);
                return Ok(pedidos);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: api/pedidos/5
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("{id}")]
        public IHttpActionResult ObtenerPorId(int id)
        {
            try
            {
                var pedido = _pedidoLN.ObtenerPorId(id);
                if (pedido == null)
                    return NotFound();

                // Verificar permisos: Admin puede ver todos, usuario solo sus pedidos
                var usuarioId = User.Identity.GetUserId();
                if (!User.IsInRole("Administrador") && pedido.UsuarioId != usuarioId)
                    return Unauthorized();

                return Ok(pedido);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT: api/pedidos/5/estado
        [System.Web.Http.HttpPut]
        [System.Web.Http.Route("{id}/estado")]
        [Authorize(Roles = "Administrador")]
        public async Task<IHttpActionResult> CambiarEstado(int id, [FromBody] CambiarEstadoRequest request)
        {
            try
            {
                if (request == null || string.IsNullOrWhiteSpace(request.Estado))
                    return BadRequest("Estado inválido");

                var resultado = await _pedidoLN.CambiarEstado(id, request.Estado);
                if (!resultado)
                    return NotFound();

                return Ok(new { mensaje = "Estado actualizado exitosamente" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: api/pedidos/estadisticas
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("estadisticas")]
        [Authorize(Roles = "Administrador")]
        public IHttpActionResult ObtenerEstadisticas()
        {
            try
            {
                var estadisticas = _pedidoLN.ObtenerEstadisticas();
                return Ok(estadisticas);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }

    // Clase auxiliar para el request de los cambios de estado
    public class CambiarEstadoRequest
    {
        public string Estado { get; set; }
    }
}