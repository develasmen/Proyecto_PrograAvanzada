using System.ComponentModel.DataAnnotations;

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
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "La marca es obligatoria.")]
        [StringLength(100)]
        public string Marca { get; set; }

        [Required]
        [Range(0.01, 1000000.00, ErrorMessage = "El precio debe ser mayor a cero.")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal Precio { get; set; }

        [Required(ErrorMessage = "El SKU es obligatorio.")]
        [StringLength(50)]
        [Display(Name = "SKU / Código de Barras")]
        public string SKU { get; set; }

        [Required]
        [Range(0, 100000, ErrorMessage = "La cantidad debe ser cero o mayor.")]
        [Display(Name = "Cantidad en Stock")]
        public int CantidadEnStock { get; set; }

        public bool Estado { get; set; }
    }
}