using Microsoft.AspNetCore.Mvc.Rendering;
using Lumina_WEB.Models;

namespace Lumina_WEB.Models.ViewModels
{
    public class UsuarioVM
    {
       
        public Usuario Usuario { get; set; } = new Usuario();

        
        public IEnumerable<SelectListItem>? CuentaLista { get; set; }
    }
}