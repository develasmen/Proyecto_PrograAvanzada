using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Inventario.Abstracciones.ModelosParaUI;
using Inventario.AccesoADatos.Modelos;

namespace Inventario.AccesoADatos.Cupon
{
    public class CuponDescuentoRepository
    {
        private readonly Contexto _contexto;

        public CuponDescuentoRepository()
        {
            _contexto = new Contexto();
        }

        public List<CuponDescuentoDto> ObtenerTodos()
        {
            return _contexto.CuponesDescuento
                .Include(c => c.Producto)
                .OrderByDescending(c => c.FechaInicio)
                .ToList()
                .Select(MapearDto)
                .ToList();
        }

        public CuponDescuentoDto ObtenerPorId(int id)
        {
            var cupon = _contexto.CuponesDescuento
                .Include(c => c.Producto)
                .FirstOrDefault(c => c.Id == id);
            return cupon == null ? null : MapearDto(cupon);
        }

        public CuponDescuentoDto ObtenerPorCodigo(string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo))
            {
                return null;
            }

            var cupon = _contexto.CuponesDescuento
                .Include(c => c.Producto)
                .FirstOrDefault(c => c.Codigo == codigo);
            return cupon == null ? null : MapearDto(cupon);
        }

        public int Guardar(CuponDescuentoDto dto)
        {
            var entidad = new CuponDescuentoAD
            {
                Codigo = dto.Codigo,
                Descripcion = dto.Descripcion,
                PorcentajeDescuento = dto.PorcentajeDescuento,
                MontoMaximo = dto.MontoMaximo,
                LimiteUso = dto.LimiteUso,
                UsosRealizados = dto.UsosRealizados,
                FechaInicio = dto.FechaInicio,
                FechaFin = dto.FechaFin,
                Activo = dto.Activo,
                ProductoId = dto.ProductoId
            };

            _contexto.CuponesDescuento.Add(entidad);
            return _contexto.SaveChanges();
        }

        public int Actualizar(CuponDescuentoDto dto)
        {
            var entidad = _contexto.CuponesDescuento.FirstOrDefault(c => c.Id == dto.Id);
            if (entidad == null)
            {
                return 0;
            }

            entidad.Codigo = dto.Codigo;
            entidad.Descripcion = dto.Descripcion;
            entidad.PorcentajeDescuento = dto.PorcentajeDescuento;
            entidad.MontoMaximo = dto.MontoMaximo;
            entidad.LimiteUso = dto.LimiteUso;
            entidad.FechaInicio = dto.FechaInicio;
            entidad.FechaFin = dto.FechaFin;
            entidad.Activo = dto.Activo;
            entidad.ProductoId = dto.ProductoId;

            _contexto.Entry(entidad).State = EntityState.Modified;
            return _contexto.SaveChanges();
        }

        public int Eliminar(int id)
        {
            var entidad = _contexto.CuponesDescuento.FirstOrDefault(c => c.Id == id);
            if (entidad == null)
            {
                return 0;
            }

            _contexto.CuponesDescuento.Remove(entidad);
            return _contexto.SaveChanges();
        }

        public int RegistrarUso(int cuponId)
        {
            var entidad = _contexto.CuponesDescuento.FirstOrDefault(c => c.Id == cuponId);
            if (entidad == null)
            {
                return 0;
            }

            entidad.UsosRealizados += 1;
            _contexto.Entry(entidad).State = EntityState.Modified;
            return _contexto.SaveChanges();
        }

        private static CuponDescuentoDto MapearDto(CuponDescuentoAD cupon)
        {
            return new CuponDescuentoDto
            {
                Id = cupon.Id,
                Codigo = cupon.Codigo,
                Descripcion = cupon.Descripcion,
                PorcentajeDescuento = cupon.PorcentajeDescuento,
                MontoMaximo = cupon.MontoMaximo,
                LimiteUso = cupon.LimiteUso,
                UsosRealizados = cupon.UsosRealizados,
                FechaInicio = cupon.FechaInicio,
                FechaFin = cupon.FechaFin,
                Activo = cupon.Activo,
                ProductoId = cupon.ProductoId,
                ProductoNombre = cupon.Producto?.Nombre
            };
        }
    }
}

