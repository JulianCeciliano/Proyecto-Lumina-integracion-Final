/*
 * Controller: Tip
 * Gestión de tips de bienestar.
 * Vista dual: admin ve CRUD completo, usuario normal solo lectura.
 * Incluye tipos de tip como categoría de cada tip.
 */
using Lumina_WEB.Datos;
using Lumina_WEB.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lumina_WEB.Controllers;

public class TipController : Controller
{
    private readonly ApplicationDbContext _context;

    public TipController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET
    public async Task<IActionResult> Index()
    {
        var tips = await _context.Tips.ToListAsync();

        ViewBag.TipoTips = await _context.TiposTip.ToListAsync();

        // Usuario normal ve solo lectura; admin ve el CRUD completo
        bool esAdmin = HttpContext.Session.GetInt32("ModoAdmin") == 2;
        return esAdmin ? View(tips) : View("IndexUsuario", tips);
    }

    // GET
    public IActionResult Create()
    {
        return View();
    }

    // POST
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Tip tip)
    {
        if (ModelState.IsValid)
        {
            _context.Tips.Add(tip);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        ViewBag.TipoTips = _context.TiposTip.ToList();
        var tips = _context.Tips.ToList();
        return View("Index", tips);
    }

    // GET
    public IActionResult Editar(int? id)
    {
        if (id == null || id == 0)
        {
            return NotFound();
        }

        var tip = _context.Tips.Find(id);

        if (tip == null)
        {
            return NotFound();
        }

        ViewBag.TipoTips = _context.TiposTip.ToList(); // agregar esta línea antes del return
        return View(tip);
    }

    // POST
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Editar(Tip tip)
    {
        if (ModelState.IsValid)
        {
            _context.Tips.Update(tip);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        return View(tip);
    }
    // GET Eliminar

    public IActionResult Eliminar(int? id)
    {
        if (id == null || id == 0)
        {
            return NotFound();
        }

        var tip = _context.Tips.Find(id);

        return View(tip);
    }

    // POST Eliminar

    [HttpPost]
    public IActionResult Eliminar(Tip tip)
    {
        if (tip == null)
        {
            return NotFound();
        }

        _context.Tips.Remove(tip);
        _context.SaveChanges();

        return RedirectToAction("Index");
    }
}
