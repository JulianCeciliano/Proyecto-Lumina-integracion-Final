/*
 * Controller: Ejercicio
 * CRUD de ejercicios de bienestar (respiración, meditación, etc.).
 * Vista dual: admin ve CRUD completo, usuario normal solo lectura.
 */
using Lumina_WEB.Datos;
using Lumina_WEB.Models;
using Microsoft.AspNetCore.Mvc;

namespace Lumina_WEB.Controllers
{

    public class EjercicioController : Controller
    {
        private readonly ApplicationDbContext _db;

        public EjercicioController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult index()
        {
            IEnumerable<Ejercicio> lista = _db.Ejercicios;

            // Usuario normal ve solo lectura; admin ve el CRUD completo
            bool esAdmin = HttpContext.Session.GetInt32("ModoAdmin") == 1;
            return esAdmin ? View(lista) : View("IndexUsuario", lista);
        }


        //GET
        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Crear(Ejercicio ejercicio)
        {
            _db.Ejercicios.Add(ejercicio);
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

            var obj = _db.Ejercicios.Find(id);

            return View(obj);
        }

        //POST EDITAR
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Editar(Ejercicio ejercicio)
        {
            if (ModelState.IsValid)
            {
                _db.Ejercicios.Update(ejercicio);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(ejercicio);
        }

        public IActionResult Eliminar(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var obj = _db.Ejercicios.Find(id);

            return View(obj);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Eliminar(Ejercicio ejercicio)
        {
            if (ejercicio == null)
            {
                return NotFound();
            }

            _db.Ejercicios.Remove(ejercicio);
            _db.SaveChanges();
            return RedirectToAction("Index");

        }



    }
}
    
    

