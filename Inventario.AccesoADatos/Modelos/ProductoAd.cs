using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inventario.AccesoADatos.Modelos
{
    [Table("Productos")]
    public class ProductoAD
    {
        [Key]
        public int Id { get; set; }
        [Required, StringLength(150)]
        public string Nombre { get; set; }
        [StringLength(500)]
        public string Descripcion { get; set; }
        [Required, StringLength(100)]
        public string Marca { get; set; }
        [Required]
        public decimal Precio { get; set; }
        [Required, StringLength(50)]
        public string SKU { get; set; }
        [Required]
        public int CantidadEnStock { get; set; }
        [Required]
        public bool Estado { get; set; }
        [Column("FECHA_REGISTRO")]
        public DateTime FechaDeRegistro { get; set; }
        [Column("FECHA_MODIFICACION")] 
        public DateTime? FechaDeModificacion { get; set; }
    }
}