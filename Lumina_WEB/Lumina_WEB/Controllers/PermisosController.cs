/*
 * Controller: Permisos
 * CRUD de permisos del sistema.
 */
using Lumina_WEB.Datos;
using Lumina_WEB.Models;
using Microsoft.AspNetCore.Mvc;

namespace Lumina_WEB.Controllers
{
    public class PermisosController : Controller
    {
        private readonly ApplicationDbContext _db;

        public PermisosController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult index()
        {
            IEnumerable<Permisos> Lista = _db.Permisos;
            return View(Lista);
            
        }

        //GET
        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Crear(Permisos permisos)
        {
            _db.Permisos.Add(permisos);
            _db.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        //GET EDITAR
        public IActionResult Editar(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var obj = _db.Permisos.Find(id);
            return View(obj);
        }

        //POST EDITAR

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Editar(Permisos permisos)
        {
            if (!ModelState.IsValid)
            {
                _db.Permisos.Update(permisos);
                _db.SaveChanges();
                return RedirectToAction("Index");

            }

            return View();
        }

        public IActionResult Eliminar(int? id)
        {
            if(id == null || id == 0)
            {
                return NotFound();
            }
            var obj = _db.Permisos.Find(id);

            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Eliminar(Permisos permisos)
        {
            if(permisos == null)
            {
                return NotFound(); 
            }

            _db.Permisos.Remove(permisos);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }


    }
}
