/*
 * Controller: Roles
 * CRUD de roles del sistema (ej: Admin, Usuario).
 */
using Microsoft.AspNetCore.Mvc;
using Lumina_WEB.Datos;
using Lumina_WEB.Models;

namespace Lumina_WEB.Controllers
{
    public class RolesController : Controller
    {
        private readonly ApplicationDbContext _db;

        public RolesController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult index()
        {
            IEnumerable<Roles> Lista = _db.Roles;
            return View(Lista);
        }

        //get
        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Crear(Roles roles)
        {
            _db.Roles.Add(roles);
            _db.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        //GET EDITAR
        public IActionResult Editar(int? id)
        {
            if(id == null || id == 0)
            {
                return NotFound();
            }

            var obj = _db.Roles.Find(id);

            return View(obj);   
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Editar(Roles roles)
        {
            if (!ModelState.IsValid)
            {
                _db.Roles.Update(roles);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(roles);
        }

        public IActionResult Eliminar(int? id)
        {
            if(id == null || id == 0)
            {
                return NotFound();
            }

            var obj = _db.Roles.Find(id);

            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Eliminar(Roles roles)
        {
            if(roles == null)
            {
                return NotFound();
            }

            _db.Roles.Remove(roles);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
