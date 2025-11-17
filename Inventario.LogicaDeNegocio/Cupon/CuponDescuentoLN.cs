using System.Collections.Generic;
using System.Threading.Tasks;
using Inventario.Abstracciones.LogicaDeNegocio.Cupon;
using Inventario.Abstracciones.ModelosParaUI;
using Inventario.AccesoADatos.Cupon;

namespace Inventario.LogicaDeNegocio.Cupon
{
    public class CuponDescuentoLN : ICuponDescuentoLN
    {
        private readonly CuponDescuentoRepository _cuponRepository;

        public CuponDescuentoLN()
        {
            _cuponRepository = new CuponDescuentoRepository();
        }

        public Task<int> Crear(CuponDescuentoDto cupon)
        {
            return Task.FromResult(_cuponRepository.Guardar(cupon));
        }

        public Task<int> Actualizar(CuponDescuentoDto cupon)
        {
            return Task.FromResult(_cuponRepository.Actualizar(cupon));
        }

        public Task<int> Eliminar(int id)
        {
            return Task.FromResult(_cuponRepository.Eliminar(id));
        }

        public Task<CuponDescuentoDto> ObtenerPorId(int id)
        {
            return Task.FromResult(_cuponRepository.ObtenerPorId(id));
        }

        public Task<CuponDescuentoDto> ObtenerPorCodigo(string codigo)
        {
            return Task.FromResult(_cuponRepository.ObtenerPorCodigo(codigo));
        }

        public Task<List<CuponDescuentoDto>> ObtenerTodos()
        {
            return Task.FromResult(_cuponRepository.ObtenerTodos());
        }

        public Task RegistrarUso(int cuponId)
        {
            _cuponRepository.RegistrarUso(cuponId);
            return Task.CompletedTask;
        }
    }
}

