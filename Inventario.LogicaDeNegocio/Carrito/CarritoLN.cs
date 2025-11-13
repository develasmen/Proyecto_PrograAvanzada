using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Inventario.Abstracciones.ModelosParaUI;
using Inventario.AccesoADatos;
using Inventario.AccesoADatos.Cupon;
using Inventario.AccesoADatos.Modelos;

namespace Inventario.LogicaDeNegocio.Carrito
{
    public class CarritoLN
    {
        private readonly Contexto _contexto;
        private readonly CuponDescuentoRepository _cuponRepository;

        public CarritoLN()
        {
            _contexto = new Contexto();
            _cuponRepository = new CuponDescuentoRepository();
        }

        public async Task<List<CarritoDto>> ObtenerCarritoPorUsuario(string usuarioId)
        {
            var itemsCarrito = await _contexto.Carritos
                .Include(c => c.Producto)
                .Include(c => c.Cupon)
                .Where(c => c.UsuarioId == usuarioId)
                .ToListAsync();

            return itemsCarrito.Select(MapearDto).ToList();
        }

        public async Task<CarritoDto> AgregarAlCarrito(string usuarioId, int productoId, int cantidad)
        {
            var producto = await _contexto.Productos.FirstOrDefaultAsync(p => p.Id == productoId);

            if (producto == null)
                throw new Exception("Producto no encontrado");

            if (!producto.Estado)
                throw new Exception("Producto no disponible");

            if (producto.CantidadEnStock < cantidad)
                throw new Exception($"Stock insuficiente. Disponible: {producto.CantidadEnStock}");

            var itemExistente = await _contexto.Carritos
                .FirstOrDefaultAsync(c => c.UsuarioId == usuarioId && c.ProductoId == productoId);

            if (itemExistente != null)
            {
                var nuevaCantidad = itemExistente.Cantidad + cantidad;
                if (producto.CantidadEnStock < nuevaCantidad)
                    throw new Exception($"Stock insuficiente. Disponible: {producto.CantidadEnStock}");

                itemExistente.Cantidad = nuevaCantidad;
                itemExistente.Subtotal = CalcularSubtotal(itemExistente.PrecioUnitario, itemExistente.Cantidad);
                itemExistente.MontoDescuento = CalcularMontoDescuento(itemExistente.Subtotal, itemExistente.PorcentajeDescuento);
            }
            else
            {
                itemExistente = new CarritoAD
                {
                    ProductoId = productoId,
                    UsuarioId = usuarioId,
                    Cantidad = cantidad,
                    PrecioUnitario = producto.Precio,
                    PorcentajeIVA = producto.PorcentajeIVA,
                    Subtotal = CalcularSubtotal(producto.Precio, cantidad),
                    PorcentajeDescuento = 0,
                    MontoDescuento = 0,
                    FechaAgregado = DateTime.Now
                };
                _contexto.Carritos.Add(itemExistente);
            }

            await _contexto.SaveChangesAsync();

            itemExistente = await _contexto.Carritos
                .Include(c => c.Producto)
                .Include(c => c.Cupon)
                .FirstOrDefaultAsync(c => c.Id == itemExistente.Id);

            return MapearDto(itemExistente);
        }

        public async Task<CarritoDto> ActualizarCantidad(int carritoId, int nuevaCantidad)
        {
            var itemCarrito = await _contexto.Carritos
                .Include(c => c.Producto)
                .Include(c => c.Cupon)
                .FirstOrDefaultAsync(c => c.Id == carritoId);

            if (itemCarrito == null)
                throw new Exception("Item no encontrado en el carrito");

            if (nuevaCantidad <= 0)
                throw new Exception("La cantidad debe ser mayor a 0");

            if (itemCarrito.Producto.CantidadEnStock < nuevaCantidad)
                throw new Exception($"Stock insuficiente. Disponible: {itemCarrito.Producto.CantidadEnStock}");

            itemCarrito.Cantidad = nuevaCantidad;
            itemCarrito.Subtotal = CalcularSubtotal(itemCarrito.PrecioUnitario, nuevaCantidad);
            itemCarrito.MontoDescuento = CalcularMontoDescuento(itemCarrito.Subtotal, itemCarrito.PorcentajeDescuento);

            await _contexto.SaveChangesAsync();

            return MapearDto(itemCarrito);
        }

        public async Task<bool> EliminarDelCarrito(int carritoId, string usuarioId)
        {
            var itemCarrito = await _contexto.Carritos
                .FirstOrDefaultAsync(c => c.Id == carritoId && c.UsuarioId == usuarioId);

            if (itemCarrito == null)
                return false;

            _contexto.Carritos.Remove(itemCarrito);
            await _contexto.SaveChangesAsync();
            return true;
        }

        public async Task<CarritoResumenDto> ObtenerResumen(string usuarioId)
        {
            var items = await _contexto.Carritos
                .Include(c => c.Cupon)
                .Where(c => c.UsuarioId == usuarioId)
                .ToListAsync();

            var subtotal = items.Sum(c => c.Subtotal);
            var totalIva = items.Sum(c => CalcularIVA(c.Subtotal, c.PorcentajeIVA));
            var totalDescuentos = items.Sum(c => c.MontoDescuento);
            var total = subtotal + totalIva - totalDescuentos;

            return new CarritoResumenDto
            {
                Subtotal = subtotal,
                TotalIVA = totalIva,
                TotalDescuentos = totalDescuentos,
                Total = decimal.Round(total, 2, MidpointRounding.AwayFromZero),
                TotalItems = items.Sum(c => c.Cantidad),
                TotalProductos = items.Count,
                CuponAplicado = string.Join(", ", items.Where(i => i.Cupon != null).Select(i => i.Cupon.Codigo).Distinct())
            };
        }

        public async Task<bool> VaciarCarrito(string usuarioId)
        {
            var items = await _contexto.Carritos
                .Where(c => c.UsuarioId == usuarioId)
                .ToListAsync();

            _contexto.Carritos.RemoveRange(items);
            await _contexto.SaveChangesAsync();
            return true;
        }

        public async Task<bool> FinalizarCompra(string usuarioId)
        {
            var itemsCarrito = await _contexto.Carritos
                .Include(c => c.Producto)
                .Where(c => c.UsuarioId == usuarioId)
                .ToListAsync();

            if (!itemsCarrito.Any())
                throw new Exception("El carrito está vacío");

            foreach (var item in itemsCarrito)
            {
                var producto = item.Producto;

                if (producto.CantidadEnStock < item.Cantidad)
                {
                    throw new Exception($"Stock insuficiente para {producto.Nombre}. Disponible: {producto.CantidadEnStock}");
                }

                producto.CantidadEnStock -= item.Cantidad;
                producto.FechaDeModificacion = DateTime.Now;
            }

            _contexto.Carritos.RemoveRange(itemsCarrito);

            await _contexto.SaveChangesAsync();

            return true;
        }

        public async Task<CarritoDto> AplicarCupon(string usuarioId, AplicarCuponRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.CodigoCupon))
            {
                throw new Exception("Debe indicar un código de cupón");
            }

            var cupon = _cuponRepository.ObtenerPorCodigo(request.CodigoCupon.Trim());
            if (cupon == null)
            {
                throw new Exception("Cupón no encontrado");
            }

            ValidarCupon(cupon);

            var item = await ObtenerItemParaCupon(usuarioId, request, cupon);
            if (item == null)
            {
                throw new Exception("No hay productos en el carrito para aplicar este cupón");
            }

            item.CuponDescuentoId = cupon.Id;
            item.PorcentajeDescuento = cupon.PorcentajeDescuento;
            item.MontoDescuento = CalcularMontoDescuento(item.Subtotal, cupon.PorcentajeDescuento, cupon.MontoMaximo);

            await _contexto.SaveChangesAsync();

            var dto = await _contexto.Carritos
                .Include(c => c.Producto)
                .Include(c => c.Cupon)
                .FirstOrDefaultAsync(c => c.Id == item.Id);

            return MapearDto(dto);
        }

        public async Task<bool> RemoverCupon(int carritoId, string usuarioId)
        {
            var item = await _contexto.Carritos
                .FirstOrDefaultAsync(c => c.Id == carritoId && c.UsuarioId == usuarioId);

            if (item == null)
            {
                return false;
            }

            item.CuponDescuentoId = null;
            item.PorcentajeDescuento = 0;
            item.MontoDescuento = 0;

            await _contexto.SaveChangesAsync();
            return true;
        }

        private async Task<CarritoAD> ObtenerItemParaCupon(string usuarioId, AplicarCuponRequest request, CuponDescuentoDto cupon)
        {
            IQueryable<CarritoAD> query = _contexto.Carritos
                .Include(c => c.Producto)
                .Include(c => c.Cupon)
                .Where(c => c.UsuarioId == usuarioId);

            if (request.CarritoId.HasValue)
            {
                query = query.Where(c => c.Id == request.CarritoId.Value);
            }
            else if (request.ProductoId.HasValue)
            {
                query = query.Where(c => c.ProductoId == request.ProductoId.Value);
            }
            else if (cupon.ProductoId.HasValue)
            {
                query = query.Where(c => c.ProductoId == cupon.ProductoId.Value);
            }

            return await query.FirstOrDefaultAsync();
        }

        private static void ValidarCupon(CuponDescuentoDto cupon)
        {
            var hoy = DateTime.Now.Date;
            if (!cupon.Activo)
            {
                throw new Exception("El cupón está inactivo");
            }

            if (cupon.FechaInicio.Date > hoy || cupon.FechaFin.Date < hoy)
            {
                throw new Exception("El cupón no se encuentra vigente");
            }

            if (cupon.LimiteUso.HasValue && cupon.UsosRealizados >= cupon.LimiteUso.Value)
            {
                throw new Exception("El cupón alcanzó el límite de usos");
            }
        }

        private static CarritoDto MapearDto(CarritoAD item)
        {
            var montoIVA = CalcularIVA(item.Subtotal, item.PorcentajeIVA);
            var totalLinea = decimal.Round(item.Subtotal + montoIVA - item.MontoDescuento, 2, MidpointRounding.AwayFromZero);

            return new CarritoDto
            {
                Id = item.Id,
                ProductoId = item.ProductoId,
                UsuarioId = item.UsuarioId,
                Cantidad = item.Cantidad,
                PrecioUnitario = item.PrecioUnitario,
                PorcentajeIVA = item.PorcentajeIVA,
                PorcentajeDescuento = item.PorcentajeDescuento,
                Subtotal = item.Subtotal,
                MontoIVA = montoIVA,
                MontoDescuento = item.MontoDescuento,
                TotalLinea = totalLinea,
                FechaAgregado = item.FechaAgregado,
                NombreProducto = item.Producto?.Nombre,
                MarcaProducto = item.Producto?.Marca,
                StockDisponible = item.Producto?.CantidadEnStock ?? 0,
                CodigoCupon = item.Cupon?.Codigo
            };
        }

        private static decimal CalcularSubtotal(decimal precioUnitario, int cantidad)
        {
            return decimal.Round(precioUnitario * cantidad, 2, MidpointRounding.AwayFromZero);
        }

        private static decimal CalcularIVA(decimal subtotal, decimal porcentajeIva)
        {
            return decimal.Round(subtotal * (porcentajeIva / 100m), 2, MidpointRounding.AwayFromZero);
        }

        private static decimal CalcularMontoDescuento(decimal subtotal, decimal porcentaje, decimal? limite = null)
        {
            if (porcentaje <= 0)
            {
                return 0;
            }

            var monto = decimal.Round(subtotal * (porcentaje / 100m), 2, MidpointRounding.AwayFromZero);

            if (limite.HasValue && limite.Value > 0)
            {
                return Math.Min(monto, limite.Value);
            }

            return monto;
        }
    }
}

