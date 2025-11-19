using InventorySystem.Context;
using InventorySystem.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace InventorySystem.Service;

public class MovimientoService
{
    private readonly AppDbContext _context;
    private readonly ILogger<MovimientoService> _logger;

    public MovimientoService(AppDbContext context, ILogger<MovimientoService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<MovimientosInventario>> GetAllMovimientosAsync()
    {
        _logger.LogInformation("Obteniendo todos los movimientos de inventario");
        var movimientos = await _context.MovimientosInventario
            .Include(m => m.Producto)
            .ThenInclude(p => p!.Categoria)
            .OrderByDescending(m => m.FechaMovimiento)
            .ToListAsync();
        _logger.LogInformation("Se obtuvieron {MovimientoCount} movimientos", movimientos.Count);
        return movimientos;
    }

    public async Task<List<MovimientosInventario>> GetUltimosMovimientosAsync(int cantidad = 10)
    {
        _logger.LogInformation("Obteniendo últimos {Cantidad} movimientos", cantidad);
        var movimientos = await _context.MovimientosInventario
            .Include(m => m.Producto)
            .ThenInclude(p => p!.Categoria)
            .OrderByDescending(m => m.FechaMovimiento)
            .Take(cantidad)
            .ToListAsync();
        _logger.LogInformation("Se obtuvieron {MovimientoCount} movimientos recientes", movimientos.Count);
        return movimientos;
    }

    public async Task<bool> RegistrarEntradaAsync(int productoId, int cantidad, string? observaciones = null)
    {
        _logger.LogInformation("Registrando entrada: ProductoID={ProductoId}, Cantidad={Cantidad}", productoId, cantidad);
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var producto = await _context.Productos.FindAsync(productoId);
            if (producto == null)
            {
                _logger.LogWarning("Producto con ID {ProductoId} no encontrado para entrada", productoId);
                return false;
            }

            var stockAnterior = producto.StockActual;

            // Crear movimiento
            var movimiento = new MovimientosInventario
            {
                ProductoID = productoId,
                TipoMovimiento = "Entrada",
                Cantidad = cantidad,
                Observaciones = observaciones,
                FechaMovimiento = DateTime.Now
            };
            _context.MovimientosInventario.Add(movimiento);

            // Actualizar stock
            producto.StockActual += cantidad;
            _context.Productos.Update(producto);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            _logger.LogInformation("Entrada registrada exitosamente: ProductoID={ProductoId}, StockAnterior={StockAnterior}, StockNuevo={StockNuevo}",
                productoId, stockAnterior, producto.StockActual);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al registrar entrada para ProductoID={ProductoId}", productoId);
            await transaction.RollbackAsync();
            return false;
        }
    }

    public async Task<bool> RegistrarSalidaAsync(int productoId, int cantidad, string? observaciones = null)
    {
        _logger.LogInformation("Registrando salida: ProductoID={ProductoId}, Cantidad={Cantidad}", productoId, cantidad);
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var producto = await _context.Productos.FindAsync(productoId);
            if (producto == null)
            {
                _logger.LogWarning("Producto con ID {ProductoId} no encontrado para salida", productoId);
                return false;
            }

            // Verificar stock suficiente
            if (producto.StockActual < cantidad)
            {
                _logger.LogWarning("Stock insuficiente para salida: ProductoID={ProductoId}, StockActual={StockActual}, CantidadSolicitada={Cantidad}",
                    productoId, producto.StockActual, cantidad);
                return false;
            }

            var stockAnterior = producto.StockActual;

            // Crear movimiento
            var movimiento = new MovimientosInventario
            {
                ProductoID = productoId,
                TipoMovimiento = "Salida",
                Cantidad = cantidad,
                Observaciones = observaciones,
                FechaMovimiento = DateTime.Now
            };
            _context.MovimientosInventario.Add(movimiento);

            // Actualizar stock
            producto.StockActual -= cantidad;
            _context.Productos.Update(producto);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            _logger.LogInformation("Salida registrada exitosamente: ProductoID={ProductoId}, StockAnterior={StockAnterior}, StockNuevo={StockNuevo}",
                productoId, stockAnterior, producto.StockActual);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al registrar salida para ProductoID={ProductoId}", productoId);
            await transaction.RollbackAsync();
            return false;
        }
    }

    public async Task<int> GetTotalMovimientosHoyAsync()
    {
        _logger.LogInformation("Obteniendo total de movimientos de hoy");
        var hoy = DateTime.Today;
        var total = await _context.MovimientosInventario
            .Where(m => m.FechaMovimiento.Date == hoy)
            .CountAsync();
        _logger.LogInformation("Total de movimientos hoy: {Total}", total);
        return total;
    }
}
