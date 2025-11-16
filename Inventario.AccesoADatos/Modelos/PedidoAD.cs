using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inventario.AccesoADatos.Modelos
{
    [Table("Pedidos")]
    public class PedidoAD
    {
        [Key]
        public int Id { get; set; }

        public int? ClienteId { get; set; }

        [Required, StringLength(128)]
        public string UsuarioId { get; set; }

        [Required]
        public DateTime Fecha { get; set; }

        [Required]
        public decimal Subtotal { get; set; }

        [Required]
        public decimal Impuestos { get; set; }

        [Required]
        public decimal Total { get; set; }

        [Required, StringLength(50)]
        public string Estado { get; set; }

        // Propiedades para la navegación
        public virtual ICollection<PedidoDetalleAD> Detalles { get; set; }

        public PedidoAD()
        {
            Detalles = new List<PedidoDetalleAD>();
            Fecha = DateTime.Now;
            Estado = "Pendiente";
        }
    }
}