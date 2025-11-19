using InventorySystem.Context;
using InventorySystem.Entities;
using Microsoft.Extensions.Logging;

namespace InventorySystem.Service;

public class ProductService
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<ProductService> _logger;

    public ProductService(AppDbContext dbContext, ILogger<ProductService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }
    public List<Productos> GetAllProducts()
    {
        _logger.LogInformation("Obteniendo lista de todos los productos");
        var products = _dbContext.Productos.ToList();
        _logger.LogInformation("Se obtuvieron {ProductCount} productos", products.Count);
        return products;
    }
    public int NewProduct(Productos product)
    {
        _logger.LogInformation("Creando nuevo producto: {ProductName}, Stock: {Stock}",
            product.Nombre, product.StockActual);
        _dbContext.Productos.Add(product);
        var result = _dbContext.SaveChanges();
        _logger.LogInformation("Producto creado exitosamente con ID: {ProductId}", product.ProductoID);
        return result;
    }

    public Productos? GetProductById(int id)
    {
        _logger.LogInformation("Buscando producto con ID: {ProductId}", id);
        var product = _dbContext.Productos.Find(id);
        if (product == null)
        {
            _logger.LogWarning("Producto con ID {ProductId} no encontrado", id);
        }
        return product;
    }

    public int UpdateProduct(Productos product)
    {
        _logger.LogInformation("Actualizando producto ID: {ProductId}, Nombre: {ProductName}, Stock: {Stock}",
            product.ProductoID, product.Nombre, product.StockActual);
        _dbContext.Productos.Update(product);
        var result = _dbContext.SaveChanges();
        _logger.LogInformation("Producto ID {ProductId} actualizado exitosamente", product.ProductoID);
        return result;
    }

    public int DeleteProduct(int id)
    {
        _logger.LogInformation("Intentando eliminar producto con ID: {ProductId}", id);
        var product = _dbContext.Productos.Find(id);
        if (product == null)
        {
            _logger.LogWarning("No se puede eliminar. Producto con ID {ProductId} no encontrado", id);
            return 0;
        }

        _dbContext.Productos.Remove(product);
        var result = _dbContext.SaveChanges();
        _logger.LogInformation("Producto ID {ProductId} eliminado exitosamente", id);
        return result;
    }
}
