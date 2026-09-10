using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lumina_WEB.Models;

public class Usuario
{
    [Key]
    public int id { get; set; }

    [ForeignKey(nameof(Cuenta))]
    public int idCuenta { get; set; }

    public string nombre { get; set; }
    public string apellido { get; set; }
    public DateTime fechaNacimiento { get; set; }
    public string urlFotoPerfil { get; set; }

    // Propiedad de navegación
    public Cuenta Cuenta { get; set; }
}