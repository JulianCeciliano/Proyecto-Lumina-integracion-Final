using System.ComponentModel.DataAnnotations;

namespace Lumina_WEB.Models
{
    public class Roles
    {
        [Key]
        public int Id { get; set; }
        public string Nombre {  get; set; }
        public string Descripcion { get; set; }
    }
}
