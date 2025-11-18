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

        public Categorias? GetCategoryById(int id)
        {
            return _dbContext.Categorias.Find(id);
        }

        public int NewCategory(Categorias category)
        {
            _dbContext.Categorias.Add(category);
            return _dbContext.SaveChanges();
        }

        public int UpdateCategory(Categorias category)
        {
            _dbContext.Categorias.Update(category);
            return _dbContext.SaveChanges();
        }

        public int DeleteCategory(int id)
        {
            var category = _dbContext.Categorias.Find(id);
            if (category == null) return 0;

            _dbContext.Categorias.Remove(category);
            return _dbContext.SaveChanges();
        }

        public bool CategoryHasProducts(int id)
        {
            return _dbContext.Productos.Any(p => p.CategoriaID == id);
        }
    }
}
