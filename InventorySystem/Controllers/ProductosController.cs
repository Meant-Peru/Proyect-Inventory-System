using InventorySystem.Entities;
using InventorySystem.Models;
using InventorySystem.Service;
using Microsoft.AspNetCore.Mvc;

namespace InventorySystem.Controllers;

public class ProductosController : Controller
{
    private readonly ProductService _serviceProduct;
    private readonly CategoryService _serviceCategory;

    public ProductosController(ProductService serviceProduct, CategoryService serviceCategory)
    {
        _serviceProduct = serviceProduct;
        _serviceCategory = serviceCategory;
    }
    public IActionResult Index()
    {
        try
        {
            var productos = _serviceProduct.GetAllProducts();
            var categorias = _serviceCategory.GetAllCategories();

            var viewModel = productos.Select(p => new ProductosViewModel
            {
                ProductoID = p.ProductoID,
                Nombre = p.Nombre,
                Descripcion = p.Descripcion,
                CategoriaNombre = categorias.FirstOrDefault(c => c.CategoriaID == p.CategoriaID)?.Nombre ?? "Sin categoria",
                StockActual = p.StockActual,
                StockMinimo = p.StockMinimo
            }).ToList();

            return View(viewModel);
        }
        catch (Exception ex)
        {
            ViewBag.Error = $"Error al cargar productos: {ex.Message}";
            return View(new List<ProductosViewModel>());
        }
    }
    [HttpGet]
    public IActionResult Create()
    {
        try
        {
            ViewBag.Categorias = _serviceCategory.GetAllCategories();
            return View();
        }
        catch (Exception ex)
        {
            ViewBag.Error = $"Error al cargar formulario: {ex.Message}";
            ViewBag.Categorias = new List<Categorias>();
            return View();
        }
    }

    [HttpPost]
    public IActionResult Create(Productos model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Categorias = _serviceCategory.GetAllCategories();
            return View(model);
        }

        var categoriaValida = _serviceCategory
            .GetAllCategories()
            .Any(c => c.CategoriaID == model.CategoriaID);

        // Validar que el CategoriaID existe en la base de datos
        if (!categoriaValida)
        {
            ModelState.AddModelError("CategoriaID", "La categoría seleccionada no es válida.");
            ViewBag.Categorias = _serviceCategory.GetAllCategories();
            return View(model);
        }

        var response = _serviceProduct.NewProduct(model);
        if(response==1) return RedirectToAction("Index");

        ViewBag.message = "Error";
        ViewBag.Categorias = _serviceCategory.GetAllCategories();
        return View(model);
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        var producto = _serviceProduct.GetProductById(id);
        if (producto == null) return NotFound();

        ViewBag.Categorias = _serviceCategory.GetAllCategories();
        return View(producto);
    }

    [HttpPost]
    public IActionResult Edit(Productos model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Categorias = _serviceCategory.GetAllCategories();
            return View(model);
        }

        var categoriaValida = _serviceCategory
            .GetAllCategories()
            .Any(c => c.CategoriaID == model.CategoriaID);

        if (!categoriaValida)
        {
            ModelState.AddModelError("CategoriaID", "La categoria seleccionada no es valida.");
            ViewBag.Categorias = _serviceCategory.GetAllCategories();
            return View(model);
        }

        var response = _serviceProduct.UpdateProduct(model);
        if (response == 1) return RedirectToAction("Index");

        ViewBag.message = "Error al actualizar";
        ViewBag.Categorias = _serviceCategory.GetAllCategories();
        return View(model);
    }

    [HttpGet]
    public IActionResult Delete(int id)
    {
        var producto = _serviceProduct.GetProductById(id);
        if (producto == null) return NotFound();

        var categorias = _serviceCategory.GetAllCategories();
        ViewBag.CategoriaNombre = categorias.FirstOrDefault(c => c.CategoriaID == producto.CategoriaID)?.Nombre ?? "Sin categoria";

        return View(producto);
    }

    [HttpPost]
    public IActionResult DeleteConfirmed(int id)
    {
        var response = _serviceProduct.DeleteProduct(id);
        if (response == 1) return RedirectToAction("Index");

        TempData["Error"] = "Error al eliminar el producto";
        return RedirectToAction("Index");
    }
}
