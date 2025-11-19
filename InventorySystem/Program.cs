using InventorySystem.Context;
using InventorySystem.Service;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configurar DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("SqlConnection");

    if (string.IsNullOrWhiteSpace(connectionString))
    {
        throw new InvalidOperationException("Connection string 'SqlConnection' is null or empty. Check environment variable 'ConnectionStrings__SqlConnection'");
    }

    options.UseSqlServer(connectionString);
    options.EnableSensitiveDataLogging(builder.Environment.IsDevelopment());
});

builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<CategoryService>();
builder.Services.AddScoped<MovimientoService>();

var app = builder.Build();

// Verificar conexión a base de datos y aplicar migraciones
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();

    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        var connectionString = builder.Configuration.GetConnectionString("SqlConnection");
        logger.LogInformation("Verificando conexion a base de datos...");
        logger.LogInformation("Connection string: Server={Server}", connectionString?.Split(';')[0]);

        // Aplicar migraciones directamente - esto manejará la conexión internamente
        logger.LogInformation("Aplicando migraciones...");
        await context.Database.MigrateAsync();
        logger.LogInformation("Migraciones aplicadas exitosamente");
        logger.LogInformation("Base de datos lista");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error al inicializar la base de datos: {Message}. La aplicación continuará pero puede tener problemas de conectividad.", ex.Message);
        // NO lanzar excepción - permitir que la app inicie y maneje errores de BD en tiempo de ejecución
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
