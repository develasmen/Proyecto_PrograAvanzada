using Inventario.Abstracciones.ModelosParaUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventario.Abstracciones.LogicaDeNegocio.Cliente
{
    public interface IClienteLN
    {
        Task<int> Guardar(ClienteDto cliente);
        Task<int> Actualizar(ClienteDto cliente);
        Task<int> Eliminar(int id);
        ClienteDto ObtenerPorId(int id);
        List<ClienteDto> ObtenerTodos();
        List<ClienteDto> BuscarPorNombreOCedula(string criterio);

    }
}
