using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Web.Mvc;

namespace Inventario.UI.Controllers
{
    public class InfoController : Controller
    {
        // GET: Info/Sucursales
        [AllowAnonymous]
        public ActionResult Sucursales()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult TerminosCondiciones()
        {
            return View();
        }

        // GET: Info/AcercaDe
        [AllowAnonymous]
        public ActionResult AcercaDe()
        {
            return View();
        }
    }
}