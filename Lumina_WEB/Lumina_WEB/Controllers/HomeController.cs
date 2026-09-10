using Lumina_WEB.Datos;
using Lumina_WEB.Models;
using Lumina_WEB.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

/*
 * Controller: Home
 * Dashboard principal del usuario.
 * Carga estados de ánimo, ejercicios sugeridos y el tip del día
 * para mostrar en la pantalla de inicio.
 */
namespace Lumina_WEB.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var vm = new HomeUsuarioViewModel
            {
                EstadosAnimo = await _context.EstadosAnimo.ToListAsync(),
                EjerciciosSugeridos = await _context.Ejercicios.Take(2).ToListAsync(),
                TipDelDia = await _context.Ejercicios.FirstOrDefaultAsync()
            };

            return View(vm);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
