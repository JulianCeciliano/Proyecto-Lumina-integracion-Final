using Lumina_WEB.Datos;
using Lumina_WEB.Models;
using Microsoft.AspNetCore.Mvc;

/*
 * Controller: Auth
 * Autenticación: Login, Registro y Cerrado de sesión.
 * Login busca la cuenta por email+password, crea la sesión
 * y redirige al Home (o a configurar perfil si es la primera vez).
 * Un doble clic en el título activa el modo administrador.
 */
namespace Lumina_WEB.Controllers;

public class AuthController : Controller
{
    private readonly ApplicationDbContext _context;

    public AuthController(ApplicationDbContext context)
    {
        _context = context;
    }
    
    /*METODOS GET Y POST DEL LOGIN*/
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    [HttpPost]
    public IActionResult Login(String email, String password, bool? modoAdmin)
    {
        var cuenta = _context.Cuentas.FirstOrDefault(c => c.Email == email && c.password == password);
        if (cuenta == null)
        {
            ViewBag.Error = "Credenciales incorrectas";
            return View();
        }

        HttpContext.Session.SetInt32("CuentaId", cuenta.Email);

        // El doble clic en el título del login activa el modo administrador
        if (modoAdmin == true)
            HttpContext.Session.SetInt32("ModoAdmin", 1);

        var usuario = _context.Usuarios.FirstOrDefault(u => u.idCuenta == cuenta.Id); // se busca el perfil asociado a esa cuenta
        if (usuario == null)
            return RedirectToAction("Create", "Usuario");// si el perfil no esta configurado, lo envia a configurarlo

        HttpContext.Session.SetInt32("UsuarioId", usuario.id); //setea la session del perfil que ingreso 
        return RedirectToAction("Index", "Home");
    }
    /*METODOS GET Y POST DEL LOGIN*/
    
    
    /*METODOS GET Y POST DEL Register*/
    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Register(Cuenta cuenta)
    {
        if (!ModelState.IsValid)
            return View(cuenta);

        bool existe = _context.Cuentas
            .Any(c => c.Email == cuenta.Email);

        if (existe)
        {
            ViewBag.Error = "El correo ya está registrado";
            return View(cuenta);
        }

        _context.Cuentas.Add(cuenta);
        _context.SaveChanges();
        HttpContext.Session.SetInt32("CuentaId", cuenta.Id);
         
        return RedirectToAction("Create", "Usuario");
    }

    /*METODO LOGOUT*/
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear(); // borra CuentaId y UsuarioId de la sesión
        return RedirectToAction("Login");
    }
}
