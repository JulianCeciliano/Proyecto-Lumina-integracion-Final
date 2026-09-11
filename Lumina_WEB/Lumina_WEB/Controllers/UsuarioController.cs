using Lumina_WEB;
using Lumina_WEB.Controllers;
using Lumina_WEB.Datos;
using Lumina_WEB.Models;
using Lumina_WEB.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

/*
 * Controller: Usuario
 * Gestión del perfil de usuario (nombre, foto, fecha de nacimiento).
 * Maneja subida y eliminación de fotos de perfil en wwwroot.
 * El perfil vincula los datos de Usuario con los de Cuenta (1:1).
 */
namespace Lumina_WEB.Controllers;

public class UsuarioController : Controller
{
    private readonly DbSet<Usuario> _context;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public UsuarioController(DbSet<Usuario> context, IWebHostEnvironment webHostEnvironment)
    {
        _context = context;
        _webHostEnvironment = webHostEnvironment;
    }

    // GET
    public IActionResult Index()
    {
        IEnumerable<Usuario> lista = _context.Usuarios;
        return View(lista);
    }

    // GET
    // GET
    public IActionResult Create()
    {
        int? cuentaId = HttpContext.Session.GetInt32("CuentaId");
        if (cuentaId == null)
            return RedirectToAction("Login", "Auth");

        var usuario = new Usuario { idCuenta = cuentaId.Value };
        return View(usuario);
    }

    // POST
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Usuario usuario, IFormFile fotoPerfil)
    {
        var cuentaId = HttpContext.Session.GetInt32("CuentaId");
        if (cuentaId == null)
            return RedirectToAction("Login", "Auth");

        usuario.idCuenta = cuentaId.Value;

        if (fotoPerfil != null)
        {
            usuario.urlFotoPerfil = await GuardarFoto(fotoPerfil);
        }
        else
        {
            usuario.urlFotoPerfil = WC.ImagenDefecto;
        }

        _context.Usuarios.Add(usuario);
        _context.SaveChanges();
        
        HttpContext.Session.SetInt32("UsuarioId", usuario.id);   // ← esto faltaba

        return RedirectToAction("Index", "Home");
    }

    //
    // GET
    //Este metodo, solo muestra los datos del perfil para despues poder editarlos
    public IActionResult Perfil()
    {
        int? usuarioId = HttpContext.Session.GetInt32("UsuarioId");
        if (usuarioId == null)
            return RedirectToAction("Login", "Auth");

        var usuario = _context.Usuarios
            .Include(u => u.Cuenta)
            .FirstOrDefault(u => u.id == usuarioId.Value);

        if (usuario == null)
            return RedirectToAction("Login", "Auth");

        var vm = new PerfilViewModel { Usuario = usuario, Cuenta = usuario.Cuenta };
        return View(vm);
    }

// POST
//El metodo post de perfil, permite editar  los datos del usuario contando los de la cuenta y del usuario, ambos 
//siendo 2 modelos que tienen una relacion 1,1 dependiendo uno del otro
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Perfil(PerfilViewModel vm, IFormFile fotoPerfil)
    {
        int? usuarioId = HttpContext.Session.GetInt32("UsuarioId");
        if (usuarioId == null)
            return RedirectToAction("Login", "Auth");

        var usuario = _context.Usuarios
            .Include(u => u.Cuenta)
            .FirstOrDefault(u => u.id == usuarioId.Value);

        if (usuario == null)
            return RedirectToAction("Login", "Auth");

        if (vm?.Usuario == null || vm.Cuenta == null)
            return RedirectToAction("Perfil");

        if (_context.Cuentas.Any(c => c.Email == vm.Cuenta.Email && c.Id != usuario.idCuenta))
        {
            TempData["Error"] = "Ese correo ya está registrado";
            return RedirectToAction("Perfil");
        }

        // ── Usuario ──
        usuario.nombre = vm.Usuario.nombre;
        usuario.apellido = vm.Usuario.apellido;
        usuario.fechaNacimiento = vm.Usuario.fechaNacimiento;
        if (fotoPerfil != null)
        {
            EliminarFoto(usuario.urlFotoPerfil);
            usuario.urlFotoPerfil = await GuardarFoto(fotoPerfil);
        }
        else if (!string.IsNullOrWhiteSpace(vm.Usuario.urlFotoPerfil))
        {
            usuario.urlFotoPerfil = vm.Usuario.urlFotoPerfil;
        }

        // ── Cuenta ──
        usuario.Cuenta.Email = vm.Cuenta.Email;
        if (!string.IsNullOrWhiteSpace(vm.Cuenta.password))
            usuario.Cuenta.password = vm.Cuenta.password;

        _context.SaveChanges();

        return RedirectToAction("Index", "Home");
    }

    private async Task<string> GuardarFoto(IFormFile foto)
    {
        string upload = _webHostEnvironment.WebRootPath + WC.ImagenRuta;
        if (!Directory.Exists(upload))
            Directory.CreateDirectory(upload);

        string fileName = Guid.NewGuid().ToString();
        string extension = Path.GetExtension(foto.FileName);

        using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
        {
            await foto.CopyToAsync(fileStream);
        }

    }

    private void EliminarFoto(string rutaVirtual)
    {
        if (string.IsNullOrWhiteSpace(rutaVirtual) || rutaVirtual == WC.ImagenDefecto)
            return;

        string rutaFisica = Path.Combine(
            _webHostEnvironment.WebRootPath,
            rutaVirtual.Replace("~/", "").Replace("/", "\\").TrimStart('\\'));

        if (System.IO.File.Exists(rutaFisica))
            System.IO.File.Delete(rutaFisica);
    }
}
