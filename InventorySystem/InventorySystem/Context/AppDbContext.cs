using InventorySystem.Entities;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Context;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options): base(options) { }

    public DbSet<Productos> Productos { get; set; }
    public DbSet<Categorias> Categorias { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Productos>()            
            .HasOne(p => p.Categoria)
            .WithMany(c => c.Productos)
            .HasForeignKey(p => p.CategoriaID)
            .OnDelete(DeleteBehavior.Restrict); // Opcional: configura el comportamiento al eliminar
        modelBuilder.Entity<Categorias>()
        .HasKey(c => c.CategoriaID);

        base.OnModelCreating(modelBuilder);
    }
}
