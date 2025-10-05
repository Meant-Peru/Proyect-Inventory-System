using InventorySystem.Context;
using InventorySystem.Entities;

namespace InventorySystem.Service;

public class ProductService
{
    private readonly AppDbContext _dbContext;
    public ProductService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public List<Productos> GetAllProducts()
    {
        return _dbContext.Productos.ToList();
    }
    public int NewProduct(Productos product)
    {
        _dbContext.Productos.Add(product);
        return _dbContext.SaveChanges();
    }   
}
