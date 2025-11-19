using System.ComponentModel.DataAnnotations;

namespace InventorySystem.Entities;

public class MovimientosInventario
{
    [Key]
    public int MovimientoID { get; set; }

    [Required]
    public int ProductoID { get; set; }

    [Required]
    public string TipoMovimiento { get; set; } = string.Empty; // "Entrada" o "Salida"

    [Required]
    public int Cantidad { get; set; }

    public string? Observaciones { get; set; }

    [Required]
    public DateTime FechaMovimiento { get; set; } = DateTime.Now;

    public Productos? Producto { get; set; }
}
