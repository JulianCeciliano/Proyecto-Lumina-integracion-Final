using System.ComponentModel.DataAnnotations;

namespace Lumina_WEB.Models
{
    public class EstadoAnimo
    {

        [Key]
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Emoji { get; set; }
        public string ColorHex { get; set; }
        public string Descripcion { get; set; }
    }
}
