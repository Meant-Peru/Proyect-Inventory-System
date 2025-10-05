
using System.ComponentModel.DataAnnotations;

namespace InventorySystem.Entities;

public class Productos
{
    [Key]
    public int ProductoID { get; set; }
    public required string Nombre { get; set; }
    public string? Descripcion { get; set; }
    public required int CategoriaID { get; set; }
    public int StockActual { get; set; } = 0;
    public int StockMinimo { get; set; } = 0;
    public Categorias? Categoria { get; set; }  
}
