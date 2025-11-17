using System;
using System.ComponentModel.DataAnnotations;

namespace Inventario.Abstracciones.ModelosParaUI
{
    public class CuponDescuentoDto
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Codigo { get; set; }

        [StringLength(200)]
        public string Descripcion { get; set; }

        [Range(0, 100)]
        [Display(Name = "Descuento (%)")]
        public decimal PorcentajeDescuento { get; set; }

        [Display(Name = "Monto maximo descuento")]
        public decimal? MontoMaximo { get; set; }

        [Display(Name = "Limite de usos")]
        public int? LimiteUso { get; set; }

        [Display(Name = "Usos acumulados")]
        public int UsosRealizados { get; set; }

        [Display(Name = "Fecha inicio")]
        public DateTime FechaInicio { get; set; }

        [Display(Name = "Fecha fin")]
        public DateTime FechaFin { get; set; }

        public bool Activo { get; set; } = true;

        [Display(Name = "Producto asociado")]
        public int? ProductoId { get; set; }

        public string ProductoNombre { get; set; }
    }
}

