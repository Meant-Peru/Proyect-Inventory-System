using InventorySystem.Entities;
using InventorySystem.Service;
using Microsoft.AspNetCore.Mvc;

namespace InventorySystem.Controllers;

public class CategoriasController : Controller
{
    private readonly CategoryService _serviceCategory;

    public CategoriasController(CategoryService serviceCategory)
    {
        _serviceCategory = serviceCategory;
    }

    public IActionResult Index()
    {
        var categorias = _serviceCategory.GetAllCategories();
        return View(categorias);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Create(Categorias model)
    {
        if (!ModelState.IsValid) return View(model);

        var response = _serviceCategory.NewCategory(model);
        if (response == 1) return RedirectToAction("Index");

        ViewBag.message = "Error al crear la categoria";
        return View();
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        var categoria = _serviceCategory.GetCategoryById(id);
        if (categoria == null) return NotFound();

        return View(categoria);
    }

    [HttpPost]
    public IActionResult Edit(Categorias model)
    {
        if (!ModelState.IsValid) return View(model);

        var response = _serviceCategory.UpdateCategory(model);
        if (response == 1) return RedirectToAction("Index");

        ViewBag.message = "Error al actualizar la categoria";
        return View(model);
    }

    [HttpGet]
    public IActionResult Delete(int id)
    {
        var categoria = _serviceCategory.GetCategoryById(id);
        if (categoria == null) return NotFound();

        if (_serviceCategory.CategoryHasProducts(id))
        {
            TempData["Error"] = "No se puede eliminar la categoria porque tiene productos asociados";
            return RedirectToAction("Index");
        }

        return View(categoria);
    }

    [HttpPost]
    public IActionResult DeleteConfirmed(int id)
    {
        if (_serviceCategory.CategoryHasProducts(id))
        {
            TempData["Error"] = "No se puede eliminar la categoria porque tiene productos asociados";
            return RedirectToAction("Index");
        }

        var response = _serviceCategory.DeleteCategory(id);
        if (response == 1) return RedirectToAction("Index");

        TempData["Error"] = "Error al eliminar la categoria";
        return RedirectToAction("Index");
    }
}
