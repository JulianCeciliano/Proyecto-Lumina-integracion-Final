/*
 * Controller: EstadoAnimo
 * CRUD de estados de ánimo del catálogo (ej: 😊 Feliz, 😢 Triste).
 * Se usa en diario para que el usuario seleccione su estado
 * y en las entradas del diario para vincular un estado.
 */
using Lumina_WEB.Models;
using Lumina_WEB.Datos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lumina_WEB.Controllers
{
    public class EstadoAnimoController : Controller
    {
        private readonly ApplicationDbContext _db;

        public EstadoAnimoController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET: EstadoAnimo
        public async Task<IActionResult> Index()
        {
            var listaOrdenada = await _db.EstadosAnimo
                .OrderBy(a => a.Nombre)
                .ToListAsync();

            return View(listaOrdenada);
        }

        // GET: EstadoAnimo/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: EstadoAnimo/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EstadoAnimo estadoAnimo)
        {
            if (!ModelState.IsValid)
            {
                return View(estadoAnimo);
            }

            _db.EstadosAnimo.Add(estadoAnimo);
            await _db.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        // GET: EstadoAnimo/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var obj = await _db.EstadosAnimo.FindAsync(id);
            if (obj == null)
            {
                return NotFound();
            }

            return View(obj);
        }

        // POST: EstadoAnimo/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EstadoAnimo estadoAnimo)
        {
            if (!ModelState.IsValid)
            {
                return View(estadoAnimo);
            }

            _db.EstadosAnimo.Update(estadoAnimo);
            await _db.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        // POST: EstadoAnimo/Delete/5
        // Borrado de un clic: se llama directo desde un <form method="post">
        // en el Index con onsubmit="return confirm(...)" en JS, sin pantalla
        // de confirmación aparte ni acción GET.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var obj = await _db.EstadosAnimo.FindAsync(id);

            if (obj != null)
            {
                _db.EstadosAnimo.Remove(obj);
                _db.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }
    }
}
