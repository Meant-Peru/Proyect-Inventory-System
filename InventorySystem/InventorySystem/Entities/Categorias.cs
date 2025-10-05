using System.ComponentModel.DataAnnotations;

namespace InventorySystem.Entities;

public class Categorias
{
    [Key]
    public int CategoriaID { get; set; }
    public required string Nombre { get; set; } = string.Empty;
    public required string? Descripcion { get; set; }

    public ICollection<Productos>? Productos { get; set; } // Propiedad de navegación inversa
}
