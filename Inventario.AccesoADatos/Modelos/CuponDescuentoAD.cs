using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inventario.AccesoADatos.Modelos
{
    [Table("CuponesDescuento")]
    public class CuponDescuentoAD
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(50)]
        public string Codigo { get; set; }

        [StringLength(200)]
        public string Descripcion { get; set; }

        [Range(0, 100)]
        public decimal PorcentajeDescuento { get; set; }

        public decimal? MontoMaximo { get; set; }

        public int? LimiteUso { get; set; }

        public int UsosRealizados { get; set; }

        public DateTime FechaInicio { get; set; }

        public DateTime FechaFin { get; set; }

        public bool Activo { get; set; }

        public int? ProductoId { get; set; }

        [ForeignKey("ProductoId")]
        public virtual ProductoAD Producto { get; set; }
    }
}

