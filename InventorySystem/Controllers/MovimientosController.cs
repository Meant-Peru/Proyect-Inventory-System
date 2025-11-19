using InventorySystem.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace InventorySystem.Controllers;

public class MovimientosController : Controller
{
    private readonly MovimientoService _movimientoService;
    private readonly ProductService _productService;

    public MovimientosController(MovimientoService movimientoService, ProductService productService)
    {
        _movimientoService = movimientoService;
        _productService = productService;
    }

    public async Task<IActionResult> Index()
    {
        var movimientos = await _movimientoService.GetAllMovimientosAsync();
        return View(movimientos);
    }

    public async Task<IActionResult> Entrada()
    {
        var productos = await _productService.GetAllProductsAsync();
        ViewBag.Productos = new SelectList(productos, "ProductoID", "Nombre");
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Entrada(int productoId, int cantidad, string? observaciones)
    {
        if (cantidad <= 0)
        {
            TempData["Error"] = "La cantidad debe ser mayor a 0";
            return RedirectToAction(nameof(Entrada));
        }

        var resultado = await _movimientoService.RegistrarEntradaAsync(productoId, cantidad, observaciones);

        if (resultado)
        {
            TempData["Success"] = "Entrada registrada exitosamente";
            return RedirectToAction(nameof(Index));
        }

        TempData["Error"] = "Error al registrar la entrada";
        return RedirectToAction(nameof(Entrada));
    }

    public async Task<IActionResult> Salida()
    {
        var productos = await _productService.GetAllProductsAsync();
        ViewBag.Productos = new SelectList(productos, "ProductoID", "Nombre");
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Salida(int productoId, int cantidad, string? observaciones)
    {
        if (cantidad <= 0)
        {
            TempData["Error"] = "La cantidad debe ser mayor a 0";
            return RedirectToAction(nameof(Salida));
        }

        var resultado = await _movimientoService.RegistrarSalidaAsync(productoId, cantidad, observaciones);

        if (resultado)
        {
            TempData["Success"] = "Salida registrada exitosamente";
            return RedirectToAction(nameof(Index));
        }

        TempData["Error"] = "Error al registrar la salida. Verifique que haya stock suficiente.";
        return RedirectToAction(nameof(Salida));
    }
}
