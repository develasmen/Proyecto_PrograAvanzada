using Inventario.AccesoADatos;
using Inventario.AccesoADatos.Modelos;
using Inventario.Abstracciones.ModelosParaUI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Inventario.LogicaDeNegocio.Carrito
{
    public class CarritoLN
    {
        private readonly Contexto _contexto;

        public CarritoLN()
        {
            _contexto = new Contexto();
        }

        public async Task<List<CarritoDto>> ObtenerCarritoPorUsuario(string usuarioId)
        {
            var itemsCarrito = await _contexto.Carritos
                .Include(c => c.Producto)
                .Where(c => c.UsuarioId == usuarioId)
                .ToListAsync();

            return itemsCarrito.Select(c => new CarritoDto
            {
                Id = c.Id,
                ProductoId = c.ProductoId,
                UsuarioId = c.UsuarioId,
                Cantidad = c.Cantidad,
                PrecioUnitario = c.PrecioUnitario,
                Subtotal = c.Subtotal,
                FechaAgregado = c.FechaAgregado,
                NombreProducto = c.Producto.Nombre,
                MarcaProducto = c.Producto.Marca,
                StockDisponible = c.Producto.CantidadEnStock
            }).ToList();
        }

        public async Task<CarritoDto> AgregarAlCarrito(string usuarioId, int productoId, int cantidad)
        {
            var producto = await _contexto.Productos.FindAsync(productoId);

            if (producto == null)
                throw new Exception("Producto no encontrado");

            if (!producto.Estado)
                throw new Exception("Producto no disponible");

            if (producto.CantidadEnStock < cantidad)
                throw new Exception($"Stock insuficiente. Disponible: {producto.CantidadEnStock}");

            // Verificar si ya existe en el carrito
            var itemExistente = await _contexto.Carritos
                .FirstOrDefaultAsync(c => c.UsuarioId == usuarioId && c.ProductoId == productoId);

            if (itemExistente != null)
            {
                // Verificar que no se exceda el stock
                int nuevaCantidad = itemExistente.Cantidad + cantidad;
                if (producto.CantidadEnStock < nuevaCantidad)
                    throw new Exception($"Stock insuficiente. Disponible: {producto.CantidadEnStock}");

                itemExistente.Cantidad = nuevaCantidad;
                itemExistente.Subtotal = itemExistente.Cantidad * itemExistente.PrecioUnitario;
            }
            else
            {
                itemExistente = new CarritoAD
                {
                    ProductoId = productoId,
                    UsuarioId = usuarioId,
                    Cantidad = cantidad,
                    PrecioUnitario = producto.Precio,
                    Subtotal = cantidad * producto.Precio,
                    FechaAgregado = DateTime.Now
                };
                _contexto.Carritos.Add(itemExistente);
            }

            await _contexto.SaveChangesAsync();

            return new CarritoDto
            {
                Id = itemExistente.Id,
                ProductoId = itemExistente.ProductoId,
                Cantidad = itemExistente.Cantidad,
                PrecioUnitario = itemExistente.PrecioUnitario,
                Subtotal = itemExistente.Subtotal,
                NombreProducto = producto.Nombre,
                MarcaProducto = producto.Marca,
                StockDisponible = producto.CantidadEnStock
            };
        }

        public async Task<CarritoDto> ActualizarCantidad(int carritoId, int nuevaCantidad)
        {
            var itemCarrito = await _contexto.Carritos
                .Include(c => c.Producto)
                .FirstOrDefaultAsync(c => c.Id == carritoId);

            if (itemCarrito == null)
                throw new Exception("Item no encontrado en el carrito");

            if (nuevaCantidad <= 0)
                throw new Exception("La cantidad debe ser mayor a 0");

            if (itemCarrito.Producto.CantidadEnStock < nuevaCantidad)
                throw new Exception($"Stock insuficiente. Disponible: {itemCarrito.Producto.CantidadEnStock}");

            itemCarrito.Cantidad = nuevaCantidad;
            itemCarrito.Subtotal = nuevaCantidad * itemCarrito.PrecioUnitario;

            await _contexto.SaveChangesAsync();

            return new CarritoDto
            {
                Id = itemCarrito.Id,
                ProductoId = itemCarrito.ProductoId,
                Cantidad = itemCarrito.Cantidad,
                PrecioUnitario = itemCarrito.PrecioUnitario,
                Subtotal = itemCarrito.Subtotal,
                NombreProducto = itemCarrito.Producto.Nombre,
                MarcaProducto = itemCarrito.Producto.Marca,
                StockDisponible = itemCarrito.Producto.CantidadEnStock
            };
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
                .Where(c => c.UsuarioId == usuarioId)
                .ToListAsync();

            return new CarritoResumenDto
            {
                Total = items.Sum(c => c.Subtotal),
                TotalItems = items.Sum(c => c.Cantidad),
                TotalProductos = items.Count
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
            // Obtener todos los items del carrito del usuario
            var itemsCarrito = await _contexto.Carritos
                .Include(c => c.Producto)
                .Where(c => c.UsuarioId == usuarioId)
                .ToListAsync();

            if (!itemsCarrito.Any())
                throw new Exception("El carrito está vacío");

            // Verificar stock y descontar
            foreach (var item in itemsCarrito)
            {
                var producto = item.Producto;

                // Verificar que hay suficiente stock
                if (producto.CantidadEnStock < item.Cantidad)
                {
                    throw new Exception($"Stock insuficiente para {producto.Nombre}. Disponible: {producto.CantidadEnStock}");
                }

                // Descontar del stock
                producto.CantidadEnStock -= item.Cantidad;
                producto.FechaDeModificacion = DateTime.Now;
            }

            // Eliminar items del carrito
            _contexto.Carritos.RemoveRange(itemsCarrito);

            // Guardar todos los cambios
            await _contexto.SaveChangesAsync();

            return true;
        }
    }
}