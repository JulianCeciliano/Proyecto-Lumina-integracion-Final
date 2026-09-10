using Lumina_WEB.Datos;
using Lumina_WEB.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

/*
 * Controller: Recordatorio
 * CRUD de recordatorios personales del usuario.
 * Cada recordatorio tiene título, descripción, fecha y estado
 * (Pendiente / Completada / Omitida).
 */
namespace Lumina_WEB.Controllers
{
    public class RecordatorioController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RecordatorioController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Recordatorio
        public async Task<IActionResult> Index()
        {
            int? usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (usuarioId == null)
                return RedirectToAction("Login", "Auth");

            var recordatorios = await _context.Recordatorios
                .Where(r => r.IdUsuario == usuarioId.Value)   // ← solo los del usuario en sesión
                .OrderBy(r => r.Fecha)
                .ToListAsync();

            return View(recordatorios);
        }

        // GET: Recordatorio/Create
        public IActionResult Create()
        {
            int? usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (usuarioId == null)
                return RedirectToAction("Login", "Auth");

            return View();
        }

        // POST: Recordatorio/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Recordatorio recordatorio)
        {
            int? usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (usuarioId == null)
                return RedirectToAction("Login", "Auth");

            recordatorio.IdUsuario = usuarioId.Value;        // ← asigna el usuario de la sesión
            recordatorio.FechaCreacion = DateTime.Now;

            ModelState.Remove(nameof(Recordatorio.IdUsuario));
            ModelState.Remove(nameof(Recordatorio.FechaCreacion));

            if (ModelState.IsValid)
            {
                return View(recordatorio);
            }

            _context.Recordatorios.Add(recordatorio);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Recordatorio/Details/5
        public async Task<IActionResult> Details(int id)
        {
            int? usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (usuarioId == null)
                return RedirectToAction("Login", "Auth");

            var recordatorio = await _context.Recordatorios
                .FirstOrDefaultAsync(r => r.Id == id && r.IdUsuario == usuarioId.Value); // ← solo suyos

            if (recordatorio == null)
            {
                return NotFound();
            }

            return View(recordatorio);
        }

        // GET: Recordatorio/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            int? usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (usuarioId == null)
                return RedirectToAction("Login", "Auth");

            var recordatorio = await _context.Recordatorios
                .FirstOrDefaultAsync(r => r.Id == id && r.IdUsuario == usuarioId.Value); // ← solo suyos

            if (recordatorio == null)
            {
                return NotFound();
            }

            return View(recordatorio);
        }

        // POST: Recordatorio/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Recordatorio recordatorio)
        {
            int? usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (usuarioId == null)
                return RedirectToAction("Login", "Auth");

            ModelState.Remove(nameof(Recordatorio.IdUsuario));
            ModelState.Remove(nameof(Recordatorio.FechaCreacion));

            if (!ModelState.IsValid)
            {
                return View(recordatorio);
            }

            var recordatorioExistente = await _context.Recordatorios
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == recordatorio.Id && r.IdUsuario == usuarioId.Value); // ← solo suyos

            if (recordatorioExistente == null)
            {
                return NotFound();
            }

            recordatorioExistente.Titulo = recordatorio.Titulo;
            recordatorioExistente.Descripcion = recordatorio.Descripcion;
            recordatorioExistente.Fecha = recordatorio.Fecha;
            recordatorioExistente.Estado = recordatorio.Estado;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // POST: Recordatorio/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            int? usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (usuarioId == null)
                return RedirectToAction("Login", "Auth");

            var recordatorio = await _context.Recordatorios
                .FirstOrDefaultAsync(r => r.Id == id && r.IdUsuario == usuarioId.Value); // ← solo suyos

            if (recordatorio == null)
            {
                return NotFound();
            }

            _context.Recordatorios.Remove(recordatorio);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
