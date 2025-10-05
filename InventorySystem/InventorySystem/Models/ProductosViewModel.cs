namespace InventorySystem.Models;

public class ProductosViewModel
{
    public int ProductoID { get; set; }
    public string Nombre { get; set; }
    public string? Descripcion { get; set; }
    public string CategoriaNombre { get; set; }
    public int StockActual { get; set; }
    public int StockMinimo { get; set; }
}
