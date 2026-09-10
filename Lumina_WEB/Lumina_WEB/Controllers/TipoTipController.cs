/*
 * Controller: TipoTip
 * CRUD de categorías de tips (ej: "Salud", "Motivación").
 * Redirige al Index de Tip después de cada operación.
 */
using Lumina_WEB.Datos;
using Lumina_WEB.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lumina_WEB.Controllers;

public class TipoTipController : Controller
{
    private readonly ApplicationDbContext _context;

    public TipoTipController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: TipoTip
    public IActionResult Index()
    {
        return RedirectToAction("Index", "Tip");
    }

    // ─────────────────────────────────────────────
    // CREATE
    // ─────────────────────────────────────────────

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(tipoTip tipoTip)
    {
        if (ModelState.IsValid)
        {
            _context.TiposTip.Add(tipoTip);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Tip");
        }

        return RedirectToAction("Index", "Tip");
    }

    // ─────────────────────────────────────────────
    // EDITAR
    // ─────────────────────────────────────────────

    // GET: TipoTip/Editar/5
    public async Task<IActionResult> Editar(int? id)
    {
        if (id == null)
            return NotFound();

        var tipoTip = await _context.TiposTip.FindAsync(id);

        if (tipoTip == null)
            return NotFound();

        return View(tipoTip);
    }

    // POST: TipoTip/Editar
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(tipoTip tipoTip)
    {
        if (!ModelState.IsValid)
        {
            return View(tipoTip);
        }

     
            _context.TiposTip.Update(tipoTip);
            await _context.SaveChangesAsync();
      

        return RedirectToAction("Index", "Tip");
    }

    // ─────────────────────────────────────────────
    // ELIMINAR
    // ─────────────────────────────────────────────

    // GET: TipoTip/Eliminar/5
    public async Task<IActionResult> Eliminar(int? id)
    {
        if (id == null)
            return NotFound();

        var tipoTip = await _context.TiposTip.FindAsync(id);

        if (tipoTip == null)
            return NotFound();

        return View(tipoTip);
    }

    // POST: TipoTip/Eliminar/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Eliminar(int id)
    {
        var tipoTip = await _context.TiposTip.FindAsync(id);

        if (tipoTip == null)
            return NotFound();

        _context.TiposTip.Remove(tipoTip);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index", "Tip");
    }
}

