using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Inventario.UI.Identity
{
    //Cree otro context para mantener la logica separada, igual usamos la misma conexion a la bd
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext() : base("name=Contexto") 
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}