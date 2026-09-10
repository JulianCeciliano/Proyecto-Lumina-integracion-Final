using System.ComponentModel.DataAnnotations;
namespace Lumina_WEB.Models;

public class tipoTip
{
    [Key]
    public int id { get; set; }

    public string nombre { get; set; }
    [MaxLength(5000)] //=====> Define la cantidad maxima de caracteres que permite el atributo
    public string descripcion { get; set; }
}