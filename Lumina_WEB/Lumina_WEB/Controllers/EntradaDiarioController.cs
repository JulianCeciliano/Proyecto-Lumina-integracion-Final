using System.Diagnostics;
using Lumina_WEB.Datos;
using Lumina_WEB.Models;
using Lumina_WEB.Servicio;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

/*
 * Controller: EntradasDiario
 * Gestiona las entradas del diario personal del usuario.
 * Cada entrada vincula un estado de ánimo con un texto libre.
 * Incluye detección automática de palabras clave de riesgo
 * que activa alertas progresivas (Leve → Media → Grande).
 */
namespace Lumina_WEB.Controllers
{
    public class EntradasDiarioController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EntradasDiarioController(ApplicationDbContext context)
        {
            _context = context;
        }

        /*
         * ACCIÓN: Index
         * Muestra únicamente las entradas del usuario en sesión
         */
        public async Task<IActionResult> Index()
        {
            int? usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (usuarioId == null)
                return RedirectToAction("Login", "Auth");

            var listaOrdenada = await _context.EntradasDiario
                .Where(e => e.IdUsuario == usuarioId.Value)   // ← solo las del usuario en sesión
                .Include(e => e.EstadoAnimo)
                .OrderByDescending(e => e.FechaCreacion)
                .ToListAsync();

            return View(listaOrdenada);
        }

        /*
         * ACCIÓN: Create (GET)
         */
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            int? usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (usuarioId == null)
                return RedirectToAction("Login", "Auth");

            ViewBag.EstadosAnimo = await _context.EstadosAnimo
                .OrderBy(e => e.Nombre)
                .ToListAsync();

            return View();
        }

        /*
         * ACCIÓN: Create (POST)
         * Asigna la entrada al usuario en sesión y la guarda
         */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EntradaDiario entrada)
        {
            int? usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (usuarioId == null)
                return RedirectToAction("Login", "Auth");

            entrada.IdUsuario = usuarioId.Value;        // ← asigna el usuario de la sesión
            entrada.FechaCreacion = DateTime.Now;

            ModelState.Remove(nameof(EntradaDiario.FechaCreacion));
            ModelState.Remove(nameof(EntradaDiario.IdUsuario));

            if (!ModelState.IsValid)
            {
                // Validacion interna para revisar el posibles errores al cargar el modelo
                //no se ven en la ejecucion del sistema
                Console.WriteLine("=== ERRORES DEL MODELSTATE ===");

                foreach (var item in ModelState)
                {
                    Console.WriteLine($"Campo: {item.Key}");

                    foreach (var error in item.Value.Errors)
                    {
                        Console.WriteLine($"  - {error.ErrorMessage}");

                        if (error.Exception != null)
                            Console.WriteLine($"  - Excepción: {error.Exception.Message}");
                    }
                }

                ViewBag.EstadosAnimo = await _context.EstadosAnimo
                    .OrderBy(e => e.Nombre)
                    .ToListAsync();

                return View(entrada);
            }

            _context.EntradasDiario.Add(entrada);
            await _context.SaveChangesAsync();

            /*
             * Detecta palabras clave: si la entrada tiene riesgo, el contador
             * acumulado sube en 1 y se devuelve la alerta que corresponde según
             * el contador (3 = Leve, 6 = Media, 9 = Grande).
             */
            var resultadoAlerta = DetectorAlertas.EvaluarYGenerarAlerta(entrada, _context.Database);

            /*
             * Si tocó una alerta, la guardamos para que el Index
             * la muestre como popup después del redirect.
             */
            if (resultadoAlerta != null)
            {
                TempData["AlertaNivel"] = resultadoAlerta.Value.Nivel;
                TempData["AlertaColor"] = resultadoAlerta.Value.Color;
                TempData["AlertaMensaje"] = resultadoAlerta.Value.Mensaje;
            }

            return RedirectToAction("Index");
        }

        /*
         * ACCIÓN: Edit (GET)
         */
        public async Task<IActionResult> Edit(int id)
        {
            int? usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (usuarioId == null)
                return RedirectToAction("Login", "Auth");

            var entrada = await _context.EntradasDiario
                .FirstOrDefaultAsync(e => e.Id == id && e.IdUsuario == usuarioId.Value); // ← solo suyas

            if (entrada == null)
            {
                return RedirectToAction("Index");
            }

            ViewBag.EstadosAnimo = await _context.EstadosAnimo
                .OrderBy(e => e.Nombre)
                .ToListAsync();

            return View(entrada);
        }

        /*
         * ACCIÓN: Edit (POST)
         */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EntradaDiario entrada)
        {
            int? usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (usuarioId == null)
                return RedirectToAction("Login", "Auth");

            ModelState.Remove(nameof(EntradaDiario.FechaCreacion));
            ModelState.Remove(nameof(EntradaDiario.IdUsuario));

            if (!ModelState.IsValid)
            {
                ViewBag.EstadosAnimo = await _context.EstadosAnimo
                    .OrderBy(e => e.Nombre)
                    .ToListAsync();

                return View(entrada);
            }

            var entradaExistente = await _context.EntradasDiario
                .FirstOrDefaultAsync(e => e.Id == entrada.Id && e.IdUsuario == usuarioId.Value); // ← solo suyas

            if (entradaExistente == null)
            {
                return RedirectToAction("Index");
            }

            entradaExistente.Titulo = entrada.Titulo;
            entradaExistente.Contenido = entrada.Contenido;
            entradaExistente.IdEstadoAnimo = entrada.IdEstadoAnimo;

            await _context.SaveChangesAsync();

            /*
             * El contenido pudo cambiar: se vuelve a evaluar. El contador del
             * sistema solo sube (no baja), así que la alerta grave se mantiene.
             */
            var resultadoAlerta = DetectorAlertas.EvaluarYGenerarAlerta(entradaExistente, _context);

            if (resultadoAlerta != null)
            {
                TempData["AlertaNivel"] = resultadoAlerta.Value.Nivel;
                TempData["AlertaColor"] = resultadoAlerta.Value.Color;
                TempData["AlertaMensaje"] = resultadoAlerta.Value.Mensaje;
            }

            return RedirectToAction("Index");
        }

        /*
         * ACCIÓN: Delete (POST)
         */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            int? usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (usuarioId == null)
                return RedirectToAction("Login", "Auth");

            var entrada = await _context.EntradasDiario
                .FirstOrDefaultAsync(e => e.Id == id && e.IdUsuario == usuarioId.Value); // ← solo suyas

            if (entrada != null)
            {
                _context.EntradasDiario.Remove(entrada);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }
    }
}
