using System;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace Inventario.Abstracciones.ModelosParaUI
{
    public class ProductoDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del producto es obligatorio.")]
        [StringLength(150)]
        [Display(Name = "Nombre del Producto")]
        public string Nombre { get; set; }

        [StringLength(500)]
        [Display(Name = "Descripcion")]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "La marca es obligatoria.")]
        [StringLength(100)]
        public string Marca { get; set; }

        [Required]
        [Range(0.01, 1000000.00, ErrorMessage = "El precio debe ser mayor a cero.")]
        [Display(Name = "Precio base")]
        public decimal Precio { get; set; }

        [Required]
        [Range(0, 100, ErrorMessage = "El IVA debe estar entre 0% y 100%.")]
        [Display(Name = "IVA (%)")]
        public decimal PorcentajeIVA { get; set; } = 13m;

        [Display(Name = "Monto IVA")]
        public decimal MontoIVA => decimal.Round(Precio * (PorcentajeIVA / 100m), 2, MidpointRounding.AwayFromZero);

        [Display(Name = "Precio con IVA")]
        public decimal PrecioConImpuestos => decimal.Round(Precio + MontoIVA, 2, MidpointRounding.AwayFromZero);

        [Required(ErrorMessage = "El SKU es obligatorio.")]
        [StringLength(50)]
        [Display(Name = "SKU / Codigo de Barras")]
        public string SKU { get; set; }

        [Required]
        [Range(0, 100000, ErrorMessage = "La cantidad debe ser cero o mayor.")]
        [Display(Name = "Cantidad en Stock")]
        public int CantidadEnStock { get; set; }

        public bool Estado { get; set; }

        // Propiedad para manejar la imagen desde la vista
        public HttpPostedFileBase archivo { get; set; }
    }
}
