using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using Workshop.Domain.Entities;
using Workshop.Infrastructure.Persistence;

namespace Workshop.Tests.Helpers
{
    /// <summary>
    /// Crea un WorkshopDbContext en memoria con nombre único por test
    /// para que cada test tenga su propia BD aislada.
    /// </summary>
    public static class DbContextFactory
    {
        public static WorkshopDbContext Create(string dbName)
        {
            var options = new DbContextOptionsBuilder<WorkshopDbContext>()
                .UseInMemoryDatabase(dbName)
                .Options;

            return new WorkshopDbContext(options);
        }

        /// <summary>
        /// Mock mínimo de UserManager — devuelve null para cualquier usuario.
        /// Suficiente para tests donde MecanicoId = null.
        /// </summary>
        public static Mock<UserManager<AppUser>> CreateUserManagerMock()
        {
            var store = new Mock<IUserStore<AppUser>>();
            var mgr = new Mock<UserManager<AppUser>>(
                store.Object, null!, null!, null!, null!, null!, null!, null!, null!);

            mgr.Setup(m => m.FindByIdAsync(It.IsAny<string>()))
               .ReturnsAsync((AppUser?)null);

            return mgr;
        }

        /// <summary>
        /// Siembra un Cliente y un Vehiculo con FK válida en la BD en memoria.
        /// InMemory aplica JOINs reales en las queries con Include, así que las FKs
        /// deben existir para que FirstAsync() devuelva resultados.
        /// </summary>
        public static async Task<(Guid clienteId, Guid vehiculoId)> SeedClienteVehiculo(
            WorkshopDbContext ctx, string placaSuffix = "")
        {
            var clienteId = Guid.NewGuid();
            var vehiculoId = Guid.NewGuid();
            var placa = "TST-" + (string.IsNullOrEmpty(placaSuffix)
                ? Guid.NewGuid().ToString("N")[..4].ToUpper()
                : placaSuffix);

            ctx.Clientes.Add(new Cliente
            {
                Id = clienteId,
                Nombre = "Cliente Test",
                Email = $"test-{clienteId}@test.com",
                Telefono = "3001234567"
            });
            ctx.Vehiculos.Add(new Vehiculo
            {
                Id = vehiculoId,
                ClienteId = clienteId,
                Placa = placa,
                Marca = "Toyota",
                Modelo = "Corolla",
                Anio = 2020
            });
            await ctx.SaveChangesAsync();

            return (clienteId, vehiculoId);
        }
    }
}
