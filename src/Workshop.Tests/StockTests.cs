using Workshop.Application.DTOs.Mantenimientos;
using Workshop.Domain.Entities;
using Workshop.Infrastructure.Persistence;
using Workshop.Infrastructure.Services;
using Workshop.Tests.Helpers;

namespace Workshop.Tests
{
    public class StockTests
    {
        /// <summary>
        /// Escenario base: un Producto con stockInicial y un Mantenimiento listo para recibir items.
        /// </summary>
        private static async Task<(MantenimientoService svc, WorkshopDbContext ctx, Guid mantenimientoId, Guid productoId)>
            CrearEscenario(string dbName, int stockInicial = 10)
        {
            var ctx = DbContextFactory.Create(dbName);
            var (clienteId, vehiculoId) = await DbContextFactory.SeedClienteVehiculo(ctx);

            var producto = new Producto
            {
                Id = Guid.NewGuid(),
                Codigo = "TEST-001",
                Nombre = "Filtro de prueba",
                Precio = 25000,
                Stock = stockInicial
            };

            var mantenimiento = new Mantenimiento
            {
                Id = Guid.NewGuid(),
                Folio = "OT-0001",
                ClienteId = clienteId,
                VehiculoId = vehiculoId,
                Estado = "Pendiente",
                Fecha = DateTime.UtcNow
            };

            ctx.Productos.Add(producto);
            ctx.Mantenimientos.Add(mantenimiento);
            await ctx.SaveChangesAsync();

            var svc = new MantenimientoService(ctx, DbContextFactory.CreateUserManagerMock().Object);
            return (svc, ctx, mantenimiento.Id, producto.Id);
        }

        // ── AddItem ──────────────────────────────────────────────────────────

        [Fact]
        public async Task AgregarItemProducto_DescuentaStock()
        {
            var (svc, ctx, mantenimientoId, productoId) =
                await CrearEscenario(nameof(AgregarItemProducto_DescuentaStock), 10);

            await svc.AddItemAsync(mantenimientoId, new AddItemDto
            {
                Tipo = "Producto",
                ProductoId = productoId,
                Cantidad = 3
            });

            Assert.Equal(7, ctx.Productos.Find(productoId)!.Stock);
        }

        [Fact]
        public async Task AgregarItemProducto_StockInsuficiente_RetornaNull()
        {
            var (svc, ctx, mantenimientoId, productoId) =
                await CrearEscenario(nameof(AgregarItemProducto_StockInsuficiente_RetornaNull), 2);

            var resultado = await svc.AddItemAsync(mantenimientoId, new AddItemDto
            {
                Tipo = "Producto",
                ProductoId = productoId,
                Cantidad = 5
            });

            Assert.Null(resultado);
        }

        [Fact]
        public async Task AgregarItemProducto_StockInsuficiente_NoModificaStock()
        {
            var (svc, ctx, mantenimientoId, productoId) =
                await CrearEscenario(nameof(AgregarItemProducto_StockInsuficiente_NoModificaStock), 2);

            await svc.AddItemAsync(mantenimientoId, new AddItemDto
            {
                Tipo = "Producto",
                ProductoId = productoId,
                Cantidad = 5
            });

            // Refrescar desde BD para asegurar valor persistido
            ctx.ChangeTracker.Clear();
            Assert.Equal(2, ctx.Productos.Find(productoId)!.Stock);
        }

        [Fact]
        public async Task AgregarItemServicio_NoTocaStock()
        {
            var (svc, ctx, mantenimientoId, productoId) =
                await CrearEscenario(nameof(AgregarItemServicio_NoTocaStock), 10);

            await svc.AddItemAsync(mantenimientoId, new AddItemDto
            {
                Tipo = "Servicio",
                Nombre = "Cambio de aceite",
                PrecioUnitario = 50000,
                Cantidad = 1
            });

            ctx.ChangeTracker.Clear();
            Assert.Equal(10, ctx.Productos.Find(productoId)!.Stock);
        }

        // ── RemoveItem ────────────────────────────────────────────────────────

        [Fact]
        public async Task QuitarItemProducto_RestauradaStock()
        {
            var (svc, ctx, mantenimientoId, productoId) =
                await CrearEscenario(nameof(QuitarItemProducto_RestauradaStock), 10);

            // Agregar item: stock baja a 8
            var orden = await svc.AddItemAsync(mantenimientoId, new AddItemDto
            {
                Tipo = "Producto",
                ProductoId = productoId,
                Cantidad = 2
            });

            var itemId = orden!.Items.First().Id;

            // Quitar item: stock debe volver a 10
            await svc.RemoveItemAsync(mantenimientoId, itemId);

            ctx.ChangeTracker.Clear();
            Assert.Equal(10, ctx.Productos.Find(productoId)!.Stock);
        }

        [Fact]
        public async Task QuitarItemServicio_NoAfectaStock()
        {
            var (svc, ctx, mantenimientoId, productoId) =
                await CrearEscenario(nameof(QuitarItemServicio_NoAfectaStock), 10);

            var orden = await svc.AddItemAsync(mantenimientoId, new AddItemDto
            {
                Tipo = "Servicio",
                Nombre = "Alineación",
                PrecioUnitario = 40000,
                Cantidad = 1
            });

            var itemId = orden!.Items.First().Id;
            await svc.RemoveItemAsync(mantenimientoId, itemId);

            ctx.ChangeTracker.Clear();
            Assert.Equal(10, ctx.Productos.Find(productoId)!.Stock);
        }
    }
}
