using Microsoft.EntityFrameworkCore;
using Lumina_WEB.Models;
namespace Lumina_WEB.Datos;

public class ApplicationDbContext : DbContext
{
    //constructor de la clase 
    public ApplicationDbContext(DbContextOptions<DbContext> options) :
        base(options) // recibe esos parametros, y con eso hace la conexion con la base de datos
    {

    }
    //por cada modelo que haya se genera un dbset

    // DbSets

    public DbSet<Alerta> Alertas { get; set; }

    public DbSet<Cuenta> Cuentas { get; set; }

    public DbSet<Recordatorio> Recordatorios { get; set; }

    public DbSet<Ejercicio> Ejercicios { get; set; }

    public DbSet<EntradaDiario> EntradasDiario { get; set; }

    public DbSet<EstadoAnimo> EstadosAnimo { get; set; }

    public DbSet<Permisos> Permisos { get; set; }

    public DbSet<Roles> Roles { get; set; }

    public DbSet<Tip> Tips { get; set; }

    public DbSet<TipoAlerta> TiposAlerta { get; set; }

    public DbSet<tipoTip> TiposTip { get; set; }

    public DbSet<Usuario> Usuarios { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<EntradaDiario>()
            .HasOne(e => e.Usuario)
            .WithMany()
            .HasForeignKey(e => e.IdUsuario)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Recordatorio>()
            .HasOne(r => r.Usuario)
            .WithMany()
            .HasForeignKey(r => r.IdUsuario)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Usuario>()
            .HasOne(u => u.Cuenta)
            .WithMany()
            .HasForeignKey(u => u.idCuenta)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
