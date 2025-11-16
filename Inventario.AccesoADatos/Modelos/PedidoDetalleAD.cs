using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inventario.AccesoADatos.Modelos
{
    [Table("PedidoDetalles")]
    public class PedidoDetalleAD
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PedidoId { get; set; }

        [Required]
        public int ProductoId { get; set; }

        [Required]
        public int Cantidad { get; set; }

        [Required]
        public decimal PrecioUnit { get; set; }

        [Required]
        public decimal Descuento { get; set; }

        [Required]
        public decimal ImpuestoPorc { get; set; }

        [Required]
        public decimal TotalLinea { get; set; }

        // Propiedades para la navegación
        [ForeignKey("PedidoId")]
        public virtual PedidoAD Pedido { get; set; }

        [ForeignKey("ProductoId")]
        public virtual ProductoAD Producto { get; set; }
    }
}