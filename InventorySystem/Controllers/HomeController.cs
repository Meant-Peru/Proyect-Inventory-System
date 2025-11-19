using System.Diagnostics;
using InventorySystem.Models;
using InventorySystem.Service;
using Microsoft.AspNetCore.Mvc;

namespace InventorySystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ProductService _productService;
        private readonly MovimientoService _movimientoService;

        public HomeController(ILogger<HomeController> logger, ProductService productService, MovimientoService movimientoService)
        {
            _logger = logger;
            _productService = productService;
            _movimientoService = movimientoService;
        }

        public IActionResult Index()
        {
            // Página simple sin acceso a BD para health checks
            ViewBag.Message = "Aplicacion ejecutandose correctamente";
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
