using Inventario.Abstracciones.ModelosParaUI;
using Inventario.AccesoADatos.Modelos;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Inventario.AccesoADatos.Producto
{
    public class ProductoRepository
    {
        private readonly Contexto _contexto;

        public ProductoRepository()
        {
            _contexto = new Contexto();
        }

        public int Guardar(ProductoDto nuevoProducto)
        {
            var productoDb = new ProductoAD()
            {
                Nombre = nuevoProducto.Nombre,
                Descripcion = nuevoProducto.Descripcion,
                Marca = nuevoProducto.Marca,
                Precio = nuevoProducto.Precio,
                SKU = nuevoProducto.SKU,
                CantidadEnStock = nuevoProducto.CantidadEnStock,
                Estado = true,
                FechaDeRegistro = DateTime.Now
            };

            _contexto.Productos.Add(productoDb);
            return _contexto.SaveChanges();
        }

        public int Actualizar(ProductoDto producto)
        {
            var productoEnDb = _contexto.Productos.FirstOrDefault(p => p.Id == producto.Id);
            if (productoEnDb != null)
            {
                productoEnDb.Nombre = producto.Nombre;
                productoEnDb.Descripcion = producto.Descripcion;
                productoEnDb.Marca = producto.Marca;
                productoEnDb.Precio = producto.Precio;
                productoEnDb.SKU = producto.SKU;
                productoEnDb.CantidadEnStock = producto.CantidadEnStock;
                productoEnDb.Estado = producto.Estado;
                productoEnDb.FechaDeModificacion = DateTime.Now;

                _contexto.Entry(productoEnDb).State = EntityState.Modified;
                return _contexto.SaveChanges();
            }
            return 0;
        }

        public int Eliminar(int id)
        {
            var productoEnDb = _contexto.Productos.FirstOrDefault(p => p.Id == id);
            if (productoEnDb != null)
            {
                _contexto.Productos.Remove(productoEnDb);
                return _contexto.SaveChanges();
            }
            return 0;
        }

        public ProductoDto ObtenerPorId(int id)
        {
            var p = _contexto.Productos.FirstOrDefault(prod => prod.Id == id);
            if (p == null) return null;

            return new ProductoDto()
            {
                Id = p.Id,
                Nombre = p.Nombre,
                Descripcion = p.Descripcion,
                Marca = p.Marca,
                Precio = p.Precio,
                SKU = p.SKU,
                CantidadEnStock = p.CantidadEnStock,
                Estado = p.Estado
            };
        }

        public List<ProductoDto> ObtenerTodos()
        {
            return _contexto.Productos.Select(p => new ProductoDto
            {
                Id = p.Id,
                Nombre = p.Nombre,
                Marca = p.Marca,
                Precio = p.Precio,
                SKU = p.SKU,
                CantidadEnStock = p.CantidadEnStock
            }).ToList();
        }
    }
}