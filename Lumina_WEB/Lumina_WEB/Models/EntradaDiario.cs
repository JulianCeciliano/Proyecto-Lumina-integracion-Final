using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lumina_WEB.Models
{
    public class EntradaDiario
    {
        [Key]
        public int Id { get; set; }

        // FK hacia Usuario (reemplaza el antiguo IdDiario)
       [ForeignKey(nameof(Usuario))]
        public int? IdUsuario { get; set; }
        public Usuario? Usuario { get; set; }

        // FK hacia EstadoAnimo (catálogo del admin)
        [ForeignKey(nameof(EstadoAnimo))]
        public int IdEstadoAnimo { get; set; }
        public EstadoAnimo? EstadoAnimo { get; set; }

        [Required]
        [MaxLength(150)]
        public string Titulo { get; set; }

        [Required]
        [StringLength(5000)]
        public string Contenido { get; set; }

        public bool ContieneAlerta { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        // Navegación 1:N hacia Alerta
        public ICollection<Alerta> Alertas { get; set; } = new List<Alerta>();
    }
}