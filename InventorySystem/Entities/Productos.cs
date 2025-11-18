using System.ComponentModel.DataAnnotations;

namespace InventorySystem.Entities;

public class Productos
{
    [Key]
    public int ProductoID { get; set; }

    [Required]
    public string Nombre { get; set; } = string.Empty;

    public string? Descripcion { get; set; }

    [Required]
    public int CategoriaID { get; set; }

    public int StockActual { get; set; } = 0;

    public int StockMinimo { get; set; } = 0;

    public Categorias? Categoria { get; set; }
}
