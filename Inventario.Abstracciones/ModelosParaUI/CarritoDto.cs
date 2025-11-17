using System;

namespace Inventario.Abstracciones.ModelosParaUI
{
    public class CarritoDto
    {
        public int Id { get; set; }
        public int ProductoId { get; set; }
        public string UsuarioId { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal PorcentajeIVA { get; set; }
        public decimal PorcentajeDescuento { get; set; }
        public decimal Subtotal { get; set; }
        public decimal MontoIVA { get; set; }
        public decimal MontoDescuento { get; set; }
        public decimal TotalLinea { get; set; }
        public DateTime FechaAgregado { get; set; }
        public string NombreProducto { get; set; }
        public string MarcaProducto { get; set; }
        public int StockDisponible { get; set; }
        public string CodigoCupon { get; set; }
    }

    public class AgregarAlCarritoRequest
    {
        public int ProductoId { get; set; }
        public int Cantidad { get; set; }
        public string CodigoCupon { get; set; }
    }

    public class ActualizarCarritoRequest
    {
        public int CarritoId { get; set; }
        public int Cantidad { get; set; }
    }

    public class AplicarCuponRequest
    {
        public int? CarritoId { get; set; }
        public int? ProductoId { get; set; }
        public string CodigoCupon { get; set; }
    }

    public class CarritoResumenDto
    {
        public decimal Subtotal { get; set; }
        public decimal TotalIVA { get; set; }
        public decimal TotalDescuentos { get; set; }
        public decimal Total { get; set; }
        public int TotalItems { get; set; }
        public int TotalProductos { get; set; }
        public string CuponAplicado { get; set; }
    }
}

