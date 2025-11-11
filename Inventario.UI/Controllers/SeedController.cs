using Inventario.UI.Identity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Inventario.UI.Controllers
{
    public class SeedController : Controller
    {
        // GET: Seed/CrearRolesYUsuarios
        public ActionResult CrearRolesYUsuarios()
        {
            var context = new ApplicationDbContext();
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            // Creamos los roles y los usuarios
            if (!roleManager.RoleExists("Administrador"))
            {
                var role = new IdentityRole("Administrador");
                roleManager.Create(role);
            }

            if (!roleManager.RoleExists("Ventas"))
            {
                var role = new IdentityRole("Ventas");
                roleManager.Create(role);
            }

            if (!roleManager.RoleExists("Operaciones"))
            {
                var role = new IdentityRole("Operaciones");
                roleManager.Create(role);
            }

            // Se crea el administrador
            var adminUser = userManager.FindByName("admin");
            if (adminUser == null)
            {
                var user = new ApplicationUser
                {
                    UserName = "admin",
                    Email = "admin@pedidos360.com",
                    NombreCompleto = "Administrador del Sistema",
                    Cedula = "000000000",
                    Direccion = "Oficina Principal",
                    FechaRegistro = DateTime.Now,
                    EmailConfirmed = true,
                    EstadoAprobacion = "Aprobado"
                };

                string password = "Admin123"; 
                var result = userManager.Create(user, password);

                if (result.Succeeded)
                {
                    userManager.AddToRole(user.Id, "Administrador");
                }
            }

            return Content("Roles: Administrador, Ventas, Operaciones creados. Usuario admin creado."); //Esta vista posibilemente la eliminemos para el ultimo avance,
                                                                                                        //toda esta info ya esta en la bd entonces si no me equivoco podriamos eliminarla
        }
    }
}