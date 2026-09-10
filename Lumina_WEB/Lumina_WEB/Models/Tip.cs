using System.ComponentModel.DataAnnotations;
namespace Lumina_WEB.Models;

public class Tip
{
    [Key]
    public int id { get; set; }
    public int idTipoTips { get; set; }
    public string nombre { get; set; }
    public string descripcion { get; set; }

}