using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventario.AccesoADatos.Modelos
{
    [Table("CLIENTES")]
    public class ClienteAd
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(200)]
        [Column("NOMBRE")]
        public string Nombre { get; set; }

        [Required, StringLength(50)]
        [Column("CEDULA_JURIDICA")]
        public string CedulaJuridica { get; set; }

        [Required, StringLength(100)]
        [Column("CORREO")]
        public string Correo { get; set; }

        [Required, StringLength(20)]
        [Column("TELEFONO")]
        public string Telefono { get; set; }

        [Required, StringLength(500)]
        [Column("DIRECCION")]
        public string Direccion { get; set; }

        [Required]
        [Column("FECHA_REGISTRO")]
        public DateTime FechaDeRegistro { get; set; }

        [Column("FECHA_MODIFICACION")]
        public DateTime? FechaDeModificacion { get; set; }

        [Required]
        [Column("ESTADO")]
        public bool Estado { get; set; }
    }
}
