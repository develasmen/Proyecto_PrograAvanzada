using Inventario.Abstracciones.ModelosParaUI;
using Inventario.AccesoADatos.Pedido;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Inventario.LogicaDeNegocio.Pedido
{
    public class PedidoLN
    {
        private readonly PedidoRepository _pedidoRepository;

        public PedidoLN()
        {
            _pedidoRepository = new PedidoRepository();
        }

        public async Task<int> CrearPedidoDesdeCarrito(string usuarioId)
        {
            return await Task.Run(() => _pedidoRepository.CrearPedidoDesdeCarrito(usuarioId));
        }

        public List<PedidoDto> ObtenerTodos()
        {
            return _pedidoRepository.ObtenerTodos();
        }

        public List<PedidoDto> ObtenerPorUsuario(string usuarioId)
        {
            return _pedidoRepository.ObtenerPorUsuario(usuarioId);
        }

        public PedidoDto ObtenerPorId(int id)
        {
            return _pedidoRepository.ObtenerPorId(id);
        }

        public async Task<bool> CambiarEstado(int pedidoId, string nuevoEstado)
        {
            return await Task.Run(() => _pedidoRepository.CambiarEstado(pedidoId, nuevoEstado));
        }

        public object ObtenerEstadisticas()
        {
            return _pedidoRepository.ObtenerEstadisticas();
        }
    }
}