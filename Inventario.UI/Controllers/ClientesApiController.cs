using Inventario.Abstracciones.ModelosParaUI;
using Inventario.LogicaDeNegocio.Cliente;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Inventario.UI.Controllers
{
    [Authorize(Roles = "Administrador")]
    [RoutePrefix("api/clientes")]
    public class ClientesApiController : ApiController
    {
        private readonly ClienteLN _clienteLN;

        public ClientesApiController()
        {
            _clienteLN = new ClienteLN();
        }

        [HttpGet]
        [Route("buscar")]
        public IHttpActionResult Buscar(string termino = "")
        {
            List<ClienteDto> clientes;

            if (string.IsNullOrWhiteSpace(termino))
            {
                clientes = _clienteLN.ObtenerTodos();
            }
            else
            {
                clientes = _clienteLN.BuscarPorNombreOCedula(termino.Trim());
            }

            return Ok(clientes.ToList());
        }
    }
}
