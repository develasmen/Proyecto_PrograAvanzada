using Inventario.UI.Identity;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

[assembly: OwinStartup(typeof(Inventario.UI.Startup))]

namespace Inventario.UI
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // configuracion del contexto de Identity
            app.CreatePerOwinContext(ApplicationDbContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);

            // Autenticacion por cookes 
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),  // ruta del login
                ExpireTimeSpan = System.TimeSpan.FromHours(2)  // la sesion no puede durar mas de 2 horas
            });
        }
    }
}