using InventorySystem.Context;
using InventorySystem.Entities;

namespace InventorySystem.Service
{
    public class CategoryService
    {
        private readonly AppDbContext _dbContext;

        public CategoryService(AppDbContext dbContext)
        {
            _dbContext = dbContext;

        }
        public List<Categorias> GetAllCategories()
        {
            return _dbContext.Categorias.ToList();
        }
    }
}
