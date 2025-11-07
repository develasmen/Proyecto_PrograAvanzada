using Inventario.Abstracciones.LogicaDeNegocio.Cliente;
using Inventario.Abstracciones.ModelosParaUI;
using Inventario.AccesoADatos.Cliente;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventario.LogicaDeNegocio.Cliente
{
    public class ClienteLN : IClienteLN
    {
        private readonly ClienteRepository _clienteRepository;

        public ClienteLN()
        {
            _clienteRepository = new ClienteRepository();
        }

        public async Task<int> Actualizar(ClienteDto cliente)
        {
            return await Task.FromResult(_clienteRepository.Actualizar(cliente));
        }

        public async Task<int> Eliminar(int id)
        {
            return await Task.FromResult(_clienteRepository.Eliminar(id));
        }

        public async Task<int> Guardar(ClienteDto cliente)
        {
            return await Task.FromResult(_clienteRepository.Guardar(cliente));
        }

        public ClienteDto ObtenerPorId(int id)
        {
            return _clienteRepository.ObtenerPorId(id);
        }

        public List<ClienteDto> ObtenerTodos()
        {
            return _clienteRepository.ObtenerTodos();
        }

        public List<ClienteDto> BuscarPorNombreOCedula(string criterio)
        {
            return _clienteRepository.BuscarPorNombreOCedula(criterio);
        }
    }
}
