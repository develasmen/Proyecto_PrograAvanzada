using Inventario.Abstracciones.ModelosParaUI;
using Inventario.AccesoADatos.Modelos;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Inventario.AccesoADatos.Pedido
{
    public class PedidoRepository
    {
        private readonly Contexto _contexto;

        public PedidoRepository()
        {
            _contexto = new Contexto();
        }

        // Crear un pedido desde el carrito del usuario
        public int CrearPedidoDesdeCarrito(string usuarioId)
        {
            // Obtener items del carrito con sus relaciones
            var itemsCarrito = _contexto.Carritos
                .Include(c => c.Producto)
                .Include(c => c.Cupon)
                .Where(c => c.UsuarioId == usuarioId)
                .ToList();

            if (!itemsCarrito.Any())
                throw new Exception("El carrito está vacío");

            // Verificar stock ANTES de procesar
            foreach (var item in itemsCarrito)
            {
                if (item.Producto.CantidadEnStock < item.Cantidad)
                    throw new Exception($"Stock insuficiente para {item.Producto.Nombre}. Disponible: {item.Producto.CantidadEnStock}");
            }

            // Calcular totales globales del pedido
            decimal subtotalPedido = 0;
            decimal totalImpuestos = 0;
            decimal totalDescuentos = 0;

            foreach (var item in itemsCarrito)
            {
                // Subtotal del item (precio * cantidad)
                decimal subtotalItem = item.Subtotal; // Ya viene calculado en el carrito

                // Calcular IVA sobre el subtotal
                decimal ivaItem = subtotalItem * (item.PorcentajeIVA / 100m);

                // Descuento del item (viene del cupón aplicado)
                decimal descuentoItem = item.MontoDescuento;

                subtotalPedido += subtotalItem;
                totalImpuestos += ivaItem;
                totalDescuentos += descuentoItem;
            }

            // Total del pedido = Subtotal + IVA - Descuentos
            decimal totalPedido = subtotalPedido + totalImpuestos - totalDescuentos;

            // Crear el pedido
            var pedido = new PedidoAD
            {
                UsuarioId = usuarioId,
                Fecha = DateTime.Now,
                Subtotal = decimal.Round(subtotalPedido, 2),
                Impuestos = decimal.Round(totalImpuestos, 2),
                Total = decimal.Round(totalPedido, 2),
                Estado = "Pendiente"
            };

            _contexto.Pedidos.Add(pedido);
            _contexto.SaveChanges();

            // Crear los detalles del pedido
            foreach (var item in itemsCarrito)
            {
                // Calcular total de esta línea (subtotal + IVA - descuento)
                decimal subtotalLinea = item.Subtotal;
                decimal ivaLinea = subtotalLinea * (item.PorcentajeIVA / 100m);
                decimal totalLinea = subtotalLinea + ivaLinea - item.MontoDescuento;

                var detalle = new PedidoDetalleAD
                {
                    PedidoId = pedido.Id,
                    ProductoId = item.ProductoId,
                    Cantidad = item.Cantidad,
                    PrecioUnit = item.PrecioUnitario,
                    Descuento = item.MontoDescuento,
                    ImpuestoPorc = item.PorcentajeIVA,
                    TotalLinea = decimal.Round(totalLinea, 2)
                };

                _contexto.PedidoDetalles.Add(detalle);

                // Descontar del stock
                var producto = item.Producto;
                producto.CantidadEnStock -= item.Cantidad;
                producto.FechaDeModificacion = DateTime.Now;

                // Incrementar contador de usos del cupón si se usó
                if (item.CuponDescuentoId.HasValue)
                {
                    var cupon = _contexto.Set<CuponDescuentoAD>().Find(item.CuponDescuentoId.Value);
                    if (cupon != null)
                    {
                        cupon.UsosRealizados++;
                    }
                }
            }

            // Vaciar el carrito del usuario
            _contexto.Carritos.RemoveRange(itemsCarrito);

            // Guardar todos los cambios en una sola transacción
            _contexto.SaveChanges();

            return pedido.Id;
        }

        // Obtener todos los pedidos
        public List<PedidoDto> ObtenerTodos()
        {
            return _contexto.Pedidos
                .OrderByDescending(p => p.Fecha)
                .Select(p => new PedidoDto
                {
                    Id = p.Id,
                    UsuarioId = p.UsuarioId,
                    Fecha = p.Fecha,
                    Subtotal = p.Subtotal,
                    Impuestos = p.Impuestos,
                    Total = p.Total,
                    Estado = p.Estado
                })
                .ToList();
        }

        // Obtener pedidos por usuario
        public List<PedidoDto> ObtenerPorUsuario(string usuarioId)
        {
            return _contexto.Pedidos
                .Where(p => p.UsuarioId == usuarioId)
                .OrderByDescending(p => p.Fecha)
                .Select(p => new PedidoDto
                {
                    Id = p.Id,
                    UsuarioId = p.UsuarioId,
                    Fecha = p.Fecha,
                    Subtotal = p.Subtotal,
                    Impuestos = p.Impuestos,
                    Total = p.Total,
                    Estado = p.Estado
                })
                .ToList();
        }

        // Obtener pedido por ID con detalles
        public PedidoDto ObtenerPorId(int id)
        {
            var pedido = _contexto.Pedidos
                .Include(p => p.Detalles)
                .Include(p => p.Detalles.Select(d => d.Producto))
                .FirstOrDefault(p => p.Id == id);

            if (pedido == null)
                return null;

            var pedidoDto = new PedidoDto
            {
                Id = pedido.Id,
                UsuarioId = pedido.UsuarioId,
                Fecha = pedido.Fecha,
                Subtotal = pedido.Subtotal,
                Impuestos = pedido.Impuestos,
                Total = pedido.Total,
                Estado = pedido.Estado,
                Detalles = pedido.Detalles.Select(d => new PedidoDetalleDto
                {
                    Id = d.Id,
                    PedidoId = d.PedidoId,
                    ProductoId = d.ProductoId,
                    NombreProducto = d.Producto.Nombre,
                    Cantidad = d.Cantidad,
                    PrecioUnit = d.PrecioUnit,
                    Descuento = d.Descuento,
                    ImpuestoPorc = d.ImpuestoPorc,
                    TotalLinea = d.TotalLinea
                }).ToList()
            };

            return pedidoDto;
        }

        // Cambiar estado de un pedido
        public bool CambiarEstado(int pedidoId, string nuevoEstado)
        {
            var pedido = _contexto.Pedidos.Find(pedidoId);
            if (pedido == null)
                return false;

            pedido.Estado = nuevoEstado;
            _contexto.SaveChanges();
            return true;
        }

        // Obtener estadísticas de pedidos
        public object ObtenerEstadisticas()
        {
            var totalPedidos = _contexto.Pedidos.Count();
            var pedidosPendientes = _contexto.Pedidos.Count(p => p.Estado == "Pendiente");
            var pedidosCompletados = _contexto.Pedidos.Count(p => p.Estado == "Completado");
            var totalVentas = _contexto.Pedidos
                .Where(p => p.Estado == "Completado")
                .Sum(p => (decimal?)p.Total) ?? 0;

            return new
            {
                TotalPedidos = totalPedidos,
                PedidosPendientes = pedidosPendientes,
                PedidosCompletados = pedidosCompletados,
                TotalVentas = totalVentas
            };
        }
    }
}