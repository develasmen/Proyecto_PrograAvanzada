using System.Web.Mvc;

public class ErrorController : Controller
{
    public ActionResult NotFound()
    {
        Response.StatusCode = 404;
        Response.TrySkipIisCustomErrors = true;
        return View("NotFound");
    }

    public ActionResult ServerError()
    {
        Response.StatusCode = 500;
        Response.TrySkipIisCustomErrors = true;
        return View("ServerError");
    }
}