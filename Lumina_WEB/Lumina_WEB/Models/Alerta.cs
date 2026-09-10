using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lumina_WEB.Models
{
    public class Alerta
    {
        [Key]
        public int Id { get; set; }

        public int IdEntrada { get; set; }

        [ForeignKey(nameof(TipoAlerta))]
        public int IdTipoAlerta { get; set; }

        public string Mensaje { get; set; }

        public DateTime FechaGenerada { get; set; } = DateTime.Now;

        public bool Atendida { get; set; }

        // Propiedad de navegación
        public TipoAlerta TipoAlerta { get; set; }
    }
}