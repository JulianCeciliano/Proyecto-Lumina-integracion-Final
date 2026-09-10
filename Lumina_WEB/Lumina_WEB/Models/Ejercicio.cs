using System.ComponentModel.DataAnnotations;
namespace Lumina_WEB.Models;

public class Ejercicio
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public string Nombre { get; set; }
    [Required]
    [MaxLength(5000)]
    public string Descripcion { get; set; }
}