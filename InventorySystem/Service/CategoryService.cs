using InventorySystem.Context;
using InventorySystem.Entities;
using Microsoft.Extensions.Logging;

namespace InventorySystem.Service
{
    public class CategoryService
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<CategoryService> _logger;

        public CategoryService(AppDbContext dbContext, ILogger<CategoryService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public List<Categorias> GetAllCategories()
        {
            _logger.LogInformation("Obteniendo lista de todas las categorías");
            var categories = _dbContext.Categorias.ToList();
            _logger.LogInformation("Se obtuvieron {CategoryCount} categorías", categories.Count);
            return categories;
        }

        public Categorias? GetCategoryById(int id)
        {
            _logger.LogInformation("Buscando categoría con ID: {CategoryId}", id);
            var category = _dbContext.Categorias.Find(id);
            if (category == null)
            {
                _logger.LogWarning("Categoría con ID {CategoryId} no encontrada", id);
            }
            return category;
        }

        public int NewCategory(Categorias category)
        {
            _logger.LogInformation("Creando nueva categoría: {CategoryName}", category.Nombre);
            _dbContext.Categorias.Add(category);
            var result = _dbContext.SaveChanges();
            _logger.LogInformation("Categoría creada exitosamente con ID: {CategoryId}", category.CategoriaID);
            return result;
        }

        public int UpdateCategory(Categorias category)
        {
            _logger.LogInformation("Actualizando categoría ID: {CategoryId}, Nombre: {CategoryName}",
                category.CategoriaID, category.Nombre);
            _dbContext.Categorias.Update(category);
            var result = _dbContext.SaveChanges();
            _logger.LogInformation("Categoría ID {CategoryId} actualizada exitosamente", category.CategoriaID);
            return result;
        }

        public int DeleteCategory(int id)
        {
            _logger.LogInformation("Intentando eliminar categoría con ID: {CategoryId}", id);
            var category = _dbContext.Categorias.Find(id);
            if (category == null)
            {
                _logger.LogWarning("No se puede eliminar. Categoría con ID {CategoryId} no encontrada", id);
                return 0;
            }

            _dbContext.Categorias.Remove(category);
            var result = _dbContext.SaveChanges();
            _logger.LogInformation("Categoría ID {CategoryId} eliminada exitosamente", id);
            return result;
        }

        public bool CategoryHasProducts(int id)
        {
            _logger.LogInformation("Verificando si categoría ID {CategoryId} tiene productos asociados", id);
            var hasProducts = _dbContext.Productos.Any(p => p.CategoriaID == id);
            _logger.LogInformation("Categoría ID {CategoryId} {HasProducts}",
                id, hasProducts ? "tiene productos asociados" : "no tiene productos asociados");
            return hasProducts;
        }
    }
}
