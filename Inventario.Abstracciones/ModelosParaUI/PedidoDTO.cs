using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Inventario.Abstracciones.ModelosParaUI
{
    public class PedidoDto
    {
        public int Id { get; set; }
        public int? ClienteId { get; set; }
        public string UsuarioId { get; set; }

        [Display(Name = "Nombre de Usuario")]
        public string NombreUsuario { get; set; }

        [Display(Name = "Fecha")]
        public DateTime Fecha { get; set; }

        [Display(Name = "Subtotal")]
        public decimal Subtotal { get; set; }

        [Display(Name = "Impuestos")]
        public decimal Impuestos { get; set; }

        [Display(Name = "Total")]
        public decimal Total { get; set; }

        [Display(Name = "Estado")]
        public string Estado { get; set; }

        public List<PedidoDetalleDto> Detalles { get; set; }

        public PedidoDto()
        {
            Detalles = new List<PedidoDetalleDto>();
        }
    }

    public class PedidoDetalleDto
    {
        public int Id { get; set; }
        public int PedidoId { get; set; }
        public int ProductoId { get; set; }

        [Display(Name = "Producto")]
        public string NombreProducto { get; set; }

        [Display(Name = "Cantidad")]
        public int Cantidad { get; set; }

        [Display(Name = "Precio Unitario")]
        public decimal PrecioUnit { get; set; }

        [Display(Name = "Descuento")]
        public decimal Descuento { get; set; }

        [Display(Name = "IVA %")]
        public decimal ImpuestoPorc { get; set; }

        [Display(Name = "Total Línea")]
        public decimal TotalLinea { get; set; }
    }

    // Este DTO es para crear un pedido desde el carrito
    public class CrearPedidoRequest
    {
        public List<int> ItemsCarritoIds { get; set; }
    }
}