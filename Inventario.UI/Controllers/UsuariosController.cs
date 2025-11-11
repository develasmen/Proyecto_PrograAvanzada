using Inventario.UI.Identity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System;

namespace Inventario.UI.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class UsuariosController : Controller
    {
        private ApplicationUserManager _userManager;
        private ApplicationDbContext _context;

        public ApplicationUserManager UserManager
        {
            get { return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>(); }
            private set { _userManager = value; }
        }

        public UsuariosController()
        {
            _context = new ApplicationDbContext();
        }

        // GET: Usuarios/UsuariosPendientes
        public ActionResult UsuariosPendientes()
        {
            var usuariosPendientes = _context.Users
                .Where(u => u.EstadoAprobacion == "Pendiente")
                .ToList();

            return View(usuariosPendientes);
        }

        // GET: Usuarios/UsuariosAprobados
        public ActionResult UsuariosAprobados()
        {
            var usuariosAprobados = _context.Users
                .Where(u => u.EstadoAprobacion == "Aprobado")
                .ToList();

            return View(usuariosAprobados);
        }

        // GET: Usuarios/AprobarUsuario/id
        public ActionResult AprobarUsuario(string id)
        {
            var usuario = _context.Users.FirstOrDefault(u => u.Id == id);

            if (usuario == null)
            {
                return HttpNotFound();
            }

            // jalamos los roles
            ViewBag.RolesDisponibles = new SelectList(new List<string> { "Ventas", "Operaciones" });

            return View(usuario);
        }

        // POST: Usuarios/AprobarUsuario
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AprobarUsuario(string id, string rolSeleccionado)
        {
            var usuario = _context.Users.FirstOrDefault(u => u.Id == id);

            if (usuario == null)
            {
                return HttpNotFound();
            }

            // asignamos de rol
            usuario.EstadoAprobacion = "Aprobado";
            _context.SaveChanges();

            // asignamos un rol (ventas o operaciones)
            await UserManager.AddToRoleAsync(usuario.Id, rolSeleccionado);

            TempData["SuccessMessage"] = $"Usuario {usuario.UserName} aprobado con rol {rolSeleccionado}.";
            return RedirectToAction("UsuariosPendientes");
        }

        // POST: Usuarios/RechazarUsuario/id
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RechazarUsuario(string id)
        {
            var usuario = _context.Users.FirstOrDefault(u => u.Id == id);

            if (usuario == null)
            {
                return HttpNotFound();
            }

            // rechazamos al usuario
            usuario.EstadoAprobacion = "Rechazado";
            _context.SaveChanges();

            TempData["SuccessMessage"] = $"Usuario {usuario.UserName} rechazado.";
            return RedirectToAction("UsuariosPendientes");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }
                if (_context != null)
                {
                    _context.Dispose();
                    _context = null;
                }
            }
            base.Dispose(disposing);
        }
    }
}