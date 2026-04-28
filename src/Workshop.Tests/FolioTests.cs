using Workshop.Application.DTOs.Mantenimientos;
using Workshop.Domain.Entities;
using Workshop.Infrastructure.Services;
using Workshop.Tests.Helpers;

namespace Workshop.Tests
{
    public class FolioTests
    {
        [Fact]
        public async Task PrimerMantenimiento_DebeGenerarFolioOT0001()
        {
            var ctx = DbContextFactory.Create(nameof(PrimerMantenimiento_DebeGenerarFolioOT0001));
            var (clienteId, vehiculoId) = await DbContextFactory.SeedClienteVehiculo(ctx);
            var svc = new MantenimientoService(ctx, DbContextFactory.CreateUserManagerMock().Object);

            var resultado = await svc.CreateAsync(new CreateMantenimientoDto
            {
                ClienteId = clienteId,
                VehiculoId = vehiculoId
            });

            Assert.Equal("OT-0001", resultado.Folio);
        }

        [Fact]
        public async Task SegundoMantenimiento_DebeGenerarFolioOT0002()
        {
            var ctx = DbContextFactory.Create(nameof(SegundoMantenimiento_DebeGenerarFolioOT0002));
            var (clienteId, vehiculoId) = await DbContextFactory.SeedClienteVehiculo(ctx);
            var svc = new MantenimientoService(ctx, DbContextFactory.CreateUserManagerMock().Object);

            var dto = new CreateMantenimientoDto { ClienteId = clienteId, VehiculoId = vehiculoId };

            await svc.CreateAsync(dto); // OT-0001
            var segundo = await svc.CreateAsync(dto); // OT-0002

            Assert.Equal("OT-0002", segundo.Folio);
        }

        [Fact]
        public async Task ConFoliosExistentes_DebeContinuarDesdeElUltimo()
        {
            var ctx = DbContextFactory.Create(nameof(ConFoliosExistentes_DebeContinuarDesdeElUltimo));
            var (clienteId, vehiculoId) = await DbContextFactory.SeedClienteVehiculo(ctx);

            // Simular órdenes ya existentes
            ctx.Mantenimientos.AddRange(
                new Mantenimiento { Id = Guid.NewGuid(), Folio = "OT-0003", ClienteId = clienteId, VehiculoId = vehiculoId, Estado = "Pendiente", Fecha = DateTime.UtcNow },
                new Mantenimiento { Id = Guid.NewGuid(), Folio = "OT-0005", ClienteId = clienteId, VehiculoId = vehiculoId, Estado = "Pendiente", Fecha = DateTime.UtcNow }
            );
            await ctx.SaveChangesAsync();

            var svc = new MantenimientoService(ctx, DbContextFactory.CreateUserManagerMock().Object);

            var resultado = await svc.CreateAsync(new CreateMantenimientoDto
            {
                ClienteId = clienteId,
                VehiculoId = vehiculoId
            });

            Assert.Equal("OT-0006", resultado.Folio);
        }

        [Fact]
        public async Task ConFoliosVaciosDeMigracionAntigua_DebeGenerarOT0001()
        {
            var ctx = DbContextFactory.Create(nameof(ConFoliosVaciosDeMigracionAntigua_DebeGenerarOT0001));
            var (clienteId, vehiculoId) = await DbContextFactory.SeedClienteVehiculo(ctx);

            // Órdenes antiguas con Folio = ""
            ctx.Mantenimientos.Add(new Mantenimiento
            {
                Id = Guid.NewGuid(),
                Folio = "",
                ClienteId = clienteId,
                VehiculoId = vehiculoId,
                Estado = "Completado",
                Fecha = DateTime.UtcNow.AddDays(-30)
            });
            await ctx.SaveChangesAsync();

            var svc = new MantenimientoService(ctx, DbContextFactory.CreateUserManagerMock().Object);

            var resultado = await svc.CreateAsync(new CreateMantenimientoDto
            {
                ClienteId = clienteId,
                VehiculoId = vehiculoId
            });

            Assert.Equal("OT-0001", resultado.Folio);
        }

        [Fact]
        public async Task NuevoMantenimiento_DebeCrearseConEstadoPendiente()
        {
            var ctx = DbContextFactory.Create(nameof(NuevoMantenimiento_DebeCrearseConEstadoPendiente));
            var (clienteId, vehiculoId) = await DbContextFactory.SeedClienteVehiculo(ctx);
            var svc = new MantenimientoService(ctx, DbContextFactory.CreateUserManagerMock().Object);

            var resultado = await svc.CreateAsync(new CreateMantenimientoDto
            {
                ClienteId = clienteId,
                VehiculoId = vehiculoId
            });

            Assert.Equal("Pendiente", resultado.Estado);
        }
    }
}
