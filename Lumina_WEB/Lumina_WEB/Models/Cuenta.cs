using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Lumina_WEB.Models;

public class Cuenta
{
    [Key]
    public int Id { get; set; }

    [Required] // ===> Hace que el atributo sea requeriado cuando se cree un objeto 
    [EmailAddress]
    public string Email { get; set; }

    public DateTime FechaCreacion { get; set; } = DateTime.Now; //====> Le asingna la fecha actual al momento de la creacion de un objeto cuenta}
   
    public string password { get; set; }
  


}