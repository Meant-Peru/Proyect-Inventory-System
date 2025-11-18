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

    public Productos? GetProductById(int id)
    {
        return _dbContext.Productos.Find(id);
    }

    public int UpdateProduct(Productos product)
    {
        _dbContext.Productos.Update(product);
        return _dbContext.SaveChanges();
    }

    public int DeleteProduct(int id)
    {
        var product = _dbContext.Productos.Find(id);
        if (product == null) return 0;

        _dbContext.Productos.Remove(product);
        return _dbContext.SaveChanges();
    }
}
