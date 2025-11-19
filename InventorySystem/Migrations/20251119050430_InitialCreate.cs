using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventorySystem.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Check and create Categorias table if not exists
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Categorias]') AND type in (N'U'))
                BEGIN
                    CREATE TABLE [Categorias] (
                        [CategoriaID] int NOT NULL IDENTITY,
                        [Nombre] nvarchar(max) NOT NULL,
                        [Descripcion] nvarchar(max) NULL,
                        CONSTRAINT [PK_Categorias] PRIMARY KEY ([CategoriaID])
                    );
                END
            ");

            // Check and create Productos table if not exists
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Productos]') AND type in (N'U'))
                BEGIN
                    CREATE TABLE [Productos] (
                        [ProductoID] int NOT NULL IDENTITY,
                        [Nombre] nvarchar(max) NOT NULL,
                        [Descripcion] nvarchar(max) NULL,
                        [CategoriaID] int NOT NULL,
                        [StockActual] int NOT NULL,
                        [StockMinimo] int NOT NULL,
                        CONSTRAINT [PK_Productos] PRIMARY KEY ([ProductoID]),
                        CONSTRAINT [FK_Productos_Categorias_CategoriaID] FOREIGN KEY ([CategoriaID]) REFERENCES [Categorias] ([CategoriaID]) ON DELETE NO ACTION
                    );
                    CREATE INDEX [IX_Productos_CategoriaID] ON [Productos] ([CategoriaID]);
                END
            ");

            // Check and create MovimientosInventario table if not exists
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[MovimientosInventario]') AND type in (N'U'))
                BEGIN
                    CREATE TABLE [MovimientosInventario] (
                        [MovimientoID] int NOT NULL IDENTITY,
                        [ProductoID] int NOT NULL,
                        [TipoMovimiento] nvarchar(max) NOT NULL,
                        [Cantidad] int NOT NULL,
                        [Observaciones] nvarchar(max) NULL,
                        [FechaMovimiento] datetime2 NOT NULL,
                        CONSTRAINT [PK_MovimientosInventario] PRIMARY KEY ([MovimientoID]),
                        CONSTRAINT [FK_MovimientosInventario_Productos_ProductoID] FOREIGN KEY ([ProductoID]) REFERENCES [Productos] ([ProductoID]) ON DELETE NO ACTION
                    );
                    CREATE INDEX [IX_MovimientosInventario_ProductoID] ON [MovimientosInventario] ([ProductoID]);
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MovimientosInventario");

            migrationBuilder.DropTable(
                name: "Productos");

            migrationBuilder.DropTable(
                name: "Categorias");
        }
    }
}
