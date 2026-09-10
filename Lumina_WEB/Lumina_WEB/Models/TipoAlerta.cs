using System.ComponentModel.DataAnnotations;

namespace Lumina_WEB.Models
{
    public class TipoAlerta
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es requerido")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "La descripción es requerida")]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "El nivel es requerido")]
        public string Nivel { get; set; } // ej: "Leve", "Media", "Grande"

        [Required(ErrorMessage = "El color es requerido")]
        public string ColorHex { get; set; }
    }
}