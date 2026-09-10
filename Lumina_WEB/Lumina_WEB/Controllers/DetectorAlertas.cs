using Lumina_WEB.Datos;
using Lumina_WEB.Models;

/*
 * Servicio: DetectorAlertas
 * Evalúa entradas del diario en busca de palabras clave de riesgo.
 * Mantene un contador acumulado por usuario (persistido en BD):
 *   3 entradas con riesgo → Alerta Leve
 *   6 entradas con riesgo → Alerta Media
 *   9+ entradas con riesgo → Alerta Grande (constante)
 */
namespace Lumina_WEB.Servicio
{
    public static class DetectorAlertas
    {
        // ADVERTENCIAAA: esta lista es solo de ejemplo para probar la lógica de conteo.
        // La lista real deberia ser un servicio o API

        private static readonly string[] PalabrasClave = new[]
        {
            "triste", "sola", "solo", "cansada", "cansado",
            "no puedo mas", "sin ganas", "ayuda"
        };

        // Umbrales del contador acumulado de entradas con riesgo:
        // al llegar a 3 se muestra la primera alerta (Leve),
        // a 6 se muestra otra (Media) y a 9 la última (Grande).
        // A partir de 9 la alerta grave se mantiene constante.
        private const int UmbralLeve = 3;
        private const int UmbralMedia = 6;
        private const int UmbralGrande = 9;

        // Colores por defecto por nivel (se usan si no hay TiposAlerta configurados)
        private static readonly Dictionary<string, string> ColoresPorNivel = new()
        {
            ["Leve"] = "#FBBF24",
            ["Media"] = "#F97316",
            ["Grande"] = "#EF4444"
        };

        public static string ObtenerColor(string nivel)
        {
            return ColoresPorNivel.TryGetValue(nivel, out var color) ? color : "#F2776E";
        }

        // Mensajes según el nivel de alerta que se muestran en el popup
        public static string ObtenerMensaje(string nivel)
        {
            return nivel switch
            {
                "Leve" => "Noto que posiblemente no estés bien. Si necesitas hablar, estoy aquí para ti.",
                "Media" => "Noto que posiblemente estés mal, necesitas ayuda externa.",
                "Grande" => "Necesitas ayuda urgente. Llama a los servicios de asistencia psicológica de Costa Rica (Línea 1322 del CCSS, atención gratuita).",
                _ => "Hay una alerta que necesita tu atención."
            };
        }

        // Cuenta cuántas veces aparecen palabras clave dentro del contenido.
        // Sirve para saber si la entrada tiene algo de riesgo.
        public static int ContarCoincidencias(string contenido)
        {
            if (string.IsNullOrWhiteSpace(contenido))
            {
                return 0;
            }

            string texto = Normalizar(contenido);
            int contador = 0;

            foreach (var palabra in PalabrasClave)
            {
                string p = Normalizar(palabra);
                int index = 0;
                while ((index = texto.IndexOf(p, index, StringComparison.Ordinal)) != -1)
                {
                    contador++;
                    index += p.Length;
                }
            }

            return contador;
        }

        // Quita tildes y acentos para que "no puedo mas" también detecte "no puedo más"
        private static string Normalizar(string texto)
        {
            var sinDiacriticos = texto.ToLowerInvariant().Normalize(System.Text.NormalizationForm.FormD);
            var resultado = new System.Text.StringBuilder();

            foreach (var c in sinDiacriticos)
            {
                if (System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c) != System.Globalization.UnicodeCategory.NonSpacingMark)
                {
                    resultado.Append(c);
                }
            }

            return resultado.ToString();
        }

        // Contador acumulado del usuario: cuántas entradas suyas tienen riesgo
        // (ContieneAlerta = true). Se calcula de la BD, así que sobrevive
        // a cerrar y reabrir el sistema.
        public static long ObtenerContador(ApplicationDbContext db, int usuarioId)
        {
            return db.EntradasDiario.Count(e => e.IdUsuario == usuarioId && e.ContieneAlerta);
        }

        // Determina el nivel de alerta según el contador acumulado.
        // Menos de 3 no muestra alerta todavía; a partir de 9 la grave es constante.
        public static string? DeterminarNivel(int contador)
        {
            if (contador >= UmbralGrande)
            {
                return "Grande";
            }
            if (contador >= UmbralMedia)
            {
                return "Media";
            }
            if (contador >= UmbralLeve)
            {
                return "Leve";
            }

            return null; // Contador menor a 3: aún no se muestra la primera alerta
        }

        // Evalúa una entrada de diario:
        //  - Si tiene palabras clave de riesgo, aumenta el contador en 1.
        //  - Según el contador acumulado (3 = Leve, 6 = Media, 9 = Grande)
        //    devuelve la alerta que el controller debe mostrar como popup.
        //  - Devuelve null si la entrada no tiene riesgo o el contador aún
        //    no llega a 3.
        public static (string Nivel, string Color, string Mensaje)? EvaluarYGenerarAlerta(EntradaDiario entrada, ApplicationDbContext db)
        {
            if (entrada.IdUsuario == null)
            {
                return null; // Defensivo: sin usuario en sesión no se cuenta
            }

            int coincidencias = ContarCoincidencias(entrada.Contenido);

            if (coincidencias == 0)
            {
                return null; // La entrada no tiene riesgo, el contador no sube
            }

            // Aumenta el contador en 1: esta entrada queda marcada como de riesgo
            if (!entrada.ContieneAlerta)
            {
                entrada.ContieneAlerta = true;
                db.Entry(entrada).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                db.SaveChanges();
            }

            // Contador acumulado de entradas con riesgo del usuario en sesión
            int contador = ObtenerContador(db, entrada.IdUsuario.Value);

            string? nivel = DeterminarNivel(contador);

            if (nivel == null)
            {
                return null; // Aún no llega a 3 coincidencias: no se muestra alerta
            }

            // El popup se muestra aunque no haya TiposAlerta configurados:
            // si no hay registro, se usa un color por defecto según el nivel.
            var tipoAlerta = db.TiposAlerta.FirstOrDefault(t => t.Nivel == nivel);
            string color = tipoAlerta != null ? tipoAlerta.ColorHex : ObtenerColor(nivel);

            // Guarda la alerta en la BD para el historial solo si hay un tipo válido
            if (tipoAlerta != null && !db.Alertas.Any(a => a.IdEntrada == entrada.Id))
            {
                var alerta = new Alerta
                {
                    IdEntrada = entrada.Id,
                    IdTipoAlerta = tipoAlerta.Id,
                    Mensaje = ObtenerMensaje(nivel),
                    FechaGenerada = DateTime.Now,
                    Atendida = false
                };

                db.Alertas.Add(alerta);
                db.SaveChanges();
            }

            return (nivel, color, ObtenerMensaje(nivel));
        }
    }
}
