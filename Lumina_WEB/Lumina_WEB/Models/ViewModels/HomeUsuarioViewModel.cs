namespace Lumina_WEB.Models.ViewModels;

public class HomeUsuarioViewModel
{
    public IEnumerable<EstadoAnimo> EstadosAnimo { get; set; }
    public IEnumerable<Ejercicio> EjerciciosSugeridos { get; set; }
    public Tip TipDelDia { get; set; }
}
