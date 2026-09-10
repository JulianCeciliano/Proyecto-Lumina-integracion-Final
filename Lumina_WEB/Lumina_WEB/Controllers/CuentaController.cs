/*
 * Controller: Cuenta
 * Administración de cuentas de usuario (solo admin).
 * Permite crear, editar y eliminar cuentas.
 * Al eliminar una cuenta, se borran en cascada su Usuario
 * y todas sus entradas del diario y recordatorios.
 */
using Lumina_WEB.Controllers;
using Lumina_WEB.Datos;
using Lumina_WEB.Models;
using Lumina_WEB.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Lumina_WEB.Controllers;

public class CuentaController : Controller
{
    private readonly ApplicationDbContext _context;

    public CuentaController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET
    public IActionResult Index()
    {
        var cuentas = _context.Cuentas.OrderBy(c => c.Id).ToList();
        var usuarios = _context.Usuarios.ToList();

        IEnumerable<PerfilViewModel> lista = cuentas.Select(c => new PerfilViewModel
        {
            Cuenta = c,
            Usuario = usuarios.FirstOrDefault(u => u.idCuenta == c.Id)
        });

        return View(lista);
    }

    // GET
    public IActionResult Create()
    {
        return View();
    }

    // POST
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Cuenta cuenta)
    {
        if (ModelState.IsValid)
        {
            _context.Cuentas.Add(cuenta);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        return View(cuenta);
    }

    // GET
    public IActionResult Editar(int? id)
    {
        if (id == null || id == 0)
        {
            return NotFound();
        }

        var cuenta = _context.Cuentas.Find(id);

        if (cuenta == null)
        {
            return NotFound();
        }

        return View(cuenta);
    }

    // POST
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Editar(Cuenta cuenta)
    {
        if (ModelState.IsValid)
        {
            _context.Cuentas.Update(cuenta);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        return View(cuenta);
    }

    // GET Eliminar
    public IActionResult Eliminar(int? id)
    {
        if (id == null || id == 0)
        {
            return NotFound();
        }

        var cuenta = _context.Cuentas.Find(id);

        return View(cuenta);
    }

    // POST Eliminar
    [HttpPost]
    public IActionResult Eliminar(Cuenta cuenta)
    {
        if (cuenta == null)
        {
            return NotFound();
        }

        _context.Cuentas.Remove(cuenta);
        _context.SaveChanges();

        return RedirectToAction("Index");
    }
}