using System.ComponentModel.DataAnnotations;

namespace InventorySystem.Entities;

public class Categorias
{
    [Key]
    public int CategoriaID { get; set; }

    [Required]
    public string Nombre { get; set; } = string.Empty;

    public string? Descripcion { get; set; }

    public ICollection<Productos>? Productos { get; set; }
}
