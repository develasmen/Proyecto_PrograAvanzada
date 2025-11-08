using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Inventario.UI.Identity
{
    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser> store)
            : base(store)
        {
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
        {
            var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(context.Get<ApplicationDbContext>()));

            // Aca validamos los requerimientos de la contraseña pero lo deje muy simple, para el proximo avance hay que valorar la seguridad de las contraseñas aunque ya esten encriptadas en la bd
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,  // min 6 caracteres
                RequireNonLetterOrDigit = false,  // sin simbolos
                RequireDigit = false,  // no requiere numeros
                RequireLowercase = false,  // no requiere minisculas
                RequireUppercase = false,  // no requiere mayusculas
            };

            // Configuracion de validacion de usuarios
            manager.UserValidator = new UserValidator<ApplicationUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,  // aca permitimos que se acepten caracteres especiales en el user
                RequireUniqueEmail = true  // El correo debe de ser unico
            };

            return manager;
        }
    }
}