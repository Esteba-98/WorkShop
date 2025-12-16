using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Workshop.Domain.Entities;

namespace Workshop.Infrastructure.Persistence
{
    public class WorkshopDbContext : IdentityDbContext<AppUser, AppRole, Guid>
    {
        public WorkshopDbContext(DbContextOptions<WorkshopDbContext> options) : base(options) { }

        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Vehiculo> Vehiculos { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Mantenimiento> Mantenimientos { get; set; }
        public DbSet<MantenimientoItem> MantenimientoItems { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Nombres personalizados de tablas Identity
            builder.Entity<AppUser>().ToTable("Usuarios");
            builder.Entity<AppRole>().ToTable("Roles");
            builder.Entity<IdentityUserRole<Guid>>().ToTable("UsuariosRoles");
            builder.Entity<IdentityUserClaim<Guid>>().ToTable("UsuariosClaims");
            builder.Entity<IdentityUserLogin<Guid>>().ToTable("UsuariosLogins");
            builder.Entity<IdentityRoleClaim<Guid>>().ToTable("RolesClaims");
            builder.Entity<IdentityUserToken<Guid>>().ToTable("UsuariosTokens");

            // Índices únicos
            builder.Entity<Cliente>()
                .HasIndex(c => c.Email)
                .IsUnique();

            builder.Entity<Vehiculo>()
                .HasIndex(v => v.Placa)
                .IsUnique();
        }
    }
}
