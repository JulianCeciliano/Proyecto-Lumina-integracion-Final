using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lumina_WEB.Models
{
    /// <summary>
    /// MODELO: Recordatorio
    /// Representa una tarea o recordatorio del usuario
    /// Ejemplos: "Estudiar cálculo", "Llamar al médico", "Sesión de respiración"
    /// 
    /// Esta clase se conecta con la tabla "Recordatorios" en la base de datos
    /// </summary>
    public class Recordatorio
    {
        /// <summary>
        /// ID único del recordatorio (clave primaria)
        /// Se genera automáticamente en la base de datos
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Nombre de usuario (ej: "@valeria_g", "@marco_a")
        /// Se muestra en la tabla de recordatorios
        /// </summary>
        //public string? Usuario  { get; set; }

        // FK hacia Usuario (un usuario tiene N recordatorios, un recordatorio pertenece a 1 usuario)
        [ForeignKey(nameof(Usuario))]
        public int? IdUsuario { get; set; }
        public Usuario? Usuario { get; set; }

        /// <summary>
        /// Título del recordatorio
        /// Ejemplo: "Estudiar cálculo", "Llamar al médico"
        /// </summary>
        public string Titulo { get; set; }

        /// <summary>
        /// Descripción detallada del recordatorio
        /// Puede ser nulo
        /// </summary>
        public string? Descripcion { get; set; }

        /// <summary>
        /// Fecha programada para el recordatorio
        /// </summary>
        public DateTime Fecha { get; set; }

        /// <summary>
        /// Estado actual del recordatorio
        /// Valores posibles: "Pendiente", "Completada", "Omitida"
        /// </summary>
        public string? Estado { get; set; }

        /// <summary>
        /// Fecha de creación del registro
        /// Se asigna automáticamente
        /// </summary>
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
    }
}