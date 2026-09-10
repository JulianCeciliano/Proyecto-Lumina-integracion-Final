/*
 * Controller: TipoAlerta
 * CRUD de tipos de alerta (Leve, Media, Grande).
 * Cada tipo define nivel, color y se vincula con las alertas
 * generadas por el DetectorAlertas.
 */
using Lumina_WEB.Models;
using Lumina_WEB.Datos;
using Microsoft.AspNetCore.Mvc;

namespace Lumina_WEB.Controllers
{
    public class TipoAlertaController : Controller
    {
        private readonly ApplicationDbContext _db;

        public TipoAlertaController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            IEnumerable<TipoAlerta> lista = _db.TiposAlerta;
            return View(lista);
        }

        //GET/CREAR
        public IActionResult Crear()
        {
            return View();
        }

        //POST/CREAR
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Crear(TipoAlerta tipoAlerta)
        {
            _db.TiposAlerta.Add(tipoAlerta);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        //METODO GET/EDITAR
        public IActionResult Editar(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var obj = _db.TiposAlerta.Find(id);
            return View(obj);
        }

        //METODO POST/EDITAR
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Editar(TipoAlerta tipoAlerta)
        {
            if (ModelState.IsValid)
            {
                _db.TiposAlerta.Update(tipoAlerta);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tipoAlerta);
        }

        //METODO GET/ELIMINAR
        public IActionResult Eliminar(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var obj = _db.TiposAlerta.Find(id);
            return View(obj);
        }

        //METODO POST/ELIMINAR
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Eliminar(TipoAlerta tipoAlerta)
        {
            if (tipoAlerta == null)
            {
                return NotFound();
            }

            _db.TiposAlerta.Remove(tipoAlerta);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}