using Inventario.LogicaDeNegocio.Pedido;
using Microsoft.AspNet.Identity;
using System.Web.Mvc;

namespace Inventario.UI.Controllers
{
    [Authorize]
    public class PedidosController : Controller
    {
        private readonly PedidoLN _pedidoLN;

        public PedidosController()
        {
            _pedidoLN = new PedidoLN();
        }

        // GET: Pedidos (Solo Admin)
        [Authorize(Roles = "Administrador")]
        public ActionResult Index()
        {
            var pedidos = _pedidoLN.ObtenerTodos();
            return View(pedidos);
        }

        // GET: Pedidos/MisPedidos (Usuario normal)
        [Authorize(Roles = "Ventas")]
        public ActionResult MisPedidos()
        {
            var usuarioId = User.Identity.GetUserId();
            var pedidos = _pedidoLN.ObtenerPorUsuario(usuarioId);
            return View(pedidos);
        }

        // GET: Pedidos/Detalle/5
        public ActionResult Detalle(int id)
        {
            var pedido = _pedidoLN.ObtenerPorId(id);

            if (pedido == null)
            {
                return HttpNotFound();
            }

            // Verificar los permisos
            var usuarioId = User.Identity.GetUserId();
            if (!User.IsInRole("Administrador") && pedido.UsuarioId != usuarioId)
            {
                return new HttpUnauthorizedResult();
            }

            return View(pedido);
        }
    }
}