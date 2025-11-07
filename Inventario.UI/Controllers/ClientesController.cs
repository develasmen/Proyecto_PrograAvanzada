using Inventario.Abstracciones.ModelosParaUI;
using Inventario.LogicaDeNegocio.Cliente;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Inventario.UI.Controllers
{
    public class ClientesController : Controller 
    {
        private readonly ClienteLN _clienteLN;

        public ClientesController()
        {
            _clienteLN = new ClienteLN();
        }

        // GET: Clientes
        public ActionResult ListarClientes()
        {
            List<ClienteDto> listaDeClientes = _clienteLN.ObtenerTodos();
            return View(listaDeClientes);
        }

        // GET: Clientes/DetallesCliente
        public ActionResult DetallesCliente(int id)
        {
            ClienteDto cliente = _clienteLN.ObtenerPorId(id);
            if (cliente == null)
            {
                return HttpNotFound();
            }
            return View(cliente);
        }

        // GET: Clientes/CrearCliente
        public ActionResult CrearCliente()
        {
            return View("CrearOEditarCliente", new ClienteDto());
        }

        // POST: Clientes/CrearCliente
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CrearCliente(ClienteDto clienteCreado)
        {
            if (ModelState.IsValid)
            {
                await _clienteLN.Guardar(clienteCreado);
                return RedirectToAction("ListarClientes");
            }
            return View("CrearOEditarCliente", clienteCreado);
        }

        // GET: Clientes/EditarCliente
        public ActionResult EditarCliente(int id)
        {
            ClienteDto cliente = _clienteLN.ObtenerPorId(id);
            if (cliente == null)
            {
                return HttpNotFound();
            }
            return View("CrearOEditarCliente", cliente);
        }

        // POST: Clientes/EditarCliente
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditarCliente(int id, ClienteDto clienteEditado)
        {
            if (ModelState.IsValid)
            {
                clienteEditado.Id = id;
                await _clienteLN.Actualizar(clienteEditado);
                return RedirectToAction("ListarClientes");
            }
            return View("CrearOEditarCliente", clienteEditado);
        }

        // GET: Clientes/EliminarCliente
        public ActionResult EliminarCliente(int id)
        {
            ClienteDto cliente = _clienteLN.ObtenerPorId(id);
            if (cliente == null)
            {
                return HttpNotFound();
            }
            return View(cliente);
        }

        // POST: Clientes/EliminarCliente
        [HttpPost, ActionName("EliminarCliente")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EliminarConfirmado(int id)
        {
            await _clienteLN.Eliminar(id);
            return RedirectToAction("ListarClientes");
        }

        // GET: Clientes/BuscarClientes
        public ActionResult BuscarClientes(string criterio)
        {
            List<ClienteDto> listaDeClientes = _clienteLN.BuscarPorNombreOCedula(criterio);
            return View("ListarClientes", listaDeClientes);
        }
    }
}