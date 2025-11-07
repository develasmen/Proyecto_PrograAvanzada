using Inventario.Abstracciones.LogicaDeNegocio.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventario.LogicaDeNegocio.General
{
	public class Fecha: IFecha
	{

		public DateTime ObtenerFechaSegunZona()
		{
			int zonaHoraria = -6;
			return DateTime.UtcNow.AddHours(zonaHoraria);
		}
	}
}
