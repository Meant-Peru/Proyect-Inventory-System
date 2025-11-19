using InventorySystem.Context;
using InventorySystem.Entities;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Service;

public class MovimientoService
{
    private readonly AppDbContext _context;

    public MovimientoService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<MovimientosInventario>> GetAllMovimientosAsync()
    {
        return await _context.MovimientosInventario
            .Include(m => m.Producto)
            .ThenInclude(p => p!.Categoria)
            .OrderByDescending(m => m.FechaMovimiento)
            .ToListAsync();
    }

    public async Task<List<MovimientosInventario>> GetUltimosMovimientosAsync(int cantidad = 10)
    {
        return await _context.MovimientosInventario
            .Include(m => m.Producto)
            .ThenInclude(p => p!.Categoria)
            .OrderByDescending(m => m.FechaMovimiento)
            .Take(cantidad)
            .ToListAsync();
    }

    public async Task<bool> RegistrarEntradaAsync(int productoId, int cantidad, string? observaciones = null)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var producto = await _context.Productos.FindAsync(productoId);
            if (producto == null) return false;

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
            return true;
        }
        catch
        {
            await transaction.RollbackAsync();
            return false;
        }
    }

    public async Task<bool> RegistrarSalidaAsync(int productoId, int cantidad, string? observaciones = null)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var producto = await _context.Productos.FindAsync(productoId);
            if (producto == null) return false;

            // Verificar stock suficiente
            if (producto.StockActual < cantidad) return false;

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
            return true;
        }
        catch
        {
            await transaction.RollbackAsync();
            return false;
        }
    }

    public async Task<int> GetTotalMovimientosHoyAsync()
    {
        var hoy = DateTime.Today;
        return await _context.MovimientosInventario
            .Where(m => m.FechaMovimiento.Date == hoy)
            .CountAsync();
    }
}
