using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inventario.AccesoADatos.Modelos
{
    [Table("Carritos")]
    public class CarritoAD
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ProductoId { get; set; }

        [Required, StringLength(256)]
        public string UsuarioId { get; set; }

        [Required]
        public int Cantidad { get; set; }

        [Required]
        public decimal PrecioUnitario { get; set; }

        [Required]
        public decimal Subtotal { get; set; }

        public DateTime FechaAgregado { get; set; }

        // Propiedades para la navegación
        [ForeignKey("ProductoId")]
        public virtual ProductoAD Producto { get; set; }
    }
}