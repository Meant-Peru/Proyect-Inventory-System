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
        var productos = _serviceProduct.GetAllProducts();
        var categorias = _serviceCategory.GetAllCategories();
        
        var viewModel= productos.Select(p => new ProductosViewModel
        {
            ProductoID = p.ProductoID,
            Nombre = p.Nombre,
            Descripcion = p.Descripcion,
            CategoriaNombre = categorias.FirstOrDefault(c => c.CategoriaID == p.CategoriaID)?.Nombre ?? "Sin categoría",
            StockActual = p.StockActual,
            StockMinimo = p.StockMinimo
        }).ToList();

        return View(viewModel);
    }
    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Create(Productos model)
    {
        

        if (!ModelState.IsValid) return View(model);

        var categoriaValida = _serviceCategory
            .GetAllCategories()
            .Any(c => c.CategoriaID == model.CategoriaID);

        // Validar que el CategoriaID existe en la base de datos
        if (!categoriaValida)
        {
            ModelState.AddModelError("CategoriaID", "La categoría seleccionada no es válida.");
            return View(model);
        }

        var response = _serviceProduct.NewProduct(model);
        if(response==1) return RedirectToAction("Index");

        ViewBag.message = "Error";
        return View();
    }
}
