using Microsoft.AspNet.Identity.EntityFramework;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Inventario.UI.Identity
{
    public class ApplicationUser : IdentityUser
    {
        //IdentityUser ya incluye username, email, passwordhash,phonenumber
        //Se agregan estas extra por ahora pero se tienen que valorar

        public string NombreCompleto { get; set; }
        public string Cedula { get; set; }
        public string Direccion { get; set; }
        public DateTime FechaRegistro { get; set; }

        public string EstadoAprobacion { get; set; }  //Cambio para aprobacion del administrador
    }
}