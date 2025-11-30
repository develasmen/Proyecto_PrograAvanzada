using System.Web.Mvc;

namespace Inventario.UI.Controllers
{
    [AllowAnonymous]
    public class ErroresController : Controller
    {
        public ActionResult Http400()
        {
            Response.StatusCode = 400;
            return View();
        }

        public ActionResult Http500()
        {
            Response.StatusCode = 500;
            return View();
        }
    }
}

