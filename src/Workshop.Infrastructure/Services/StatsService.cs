using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Workshop.Application.DTOs.Stats;
using Workshop.Application.Services;
using Workshop.Domain.Entities;
using Workshop.Infrastructure.Persistence;

namespace Workshop.Infrastructure.Services
{
    public class StatsService : IStatsService
    {
        private readonly WorkshopDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public StatsService(WorkshopDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<DashboardStatsDto> GetDashboardStatsAsync(string periodo = "mes")
        {
            var now = DateTime.UtcNow;
            var inicioPeriodo = periodo switch
            {
                "semana" => now.AddDays(-7),
                "anio"   => new DateTime(now.Year, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                "todo"   => DateTime.MinValue,
                _        => new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc)
            };
            var inicioMes = inicioPeriodo; // alias semántico para los campos del DTO
            var inicioHoy = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0, DateTimeKind.Utc);

            var mantenimientos = await _context.Mantenimientos
                .Include(m => m.Items)
                .ToListAsync();

            var totalClientes = await _context.Clientes.CountAsync();
            var totalVehiculos = await _context.Vehiculos.CountAsync();
            var productos = await _context.Productos.ToListAsync();

            var ordenesMes = mantenimientos.Count(m => m.Fecha >= inicioMes);
            var ordenesHoy = mantenimientos.Count(m => m.Fecha >= inicioHoy);

            var ingresosMes = mantenimientos
                .Where(m => m.Estado == "Completado" && m.Fecha >= inicioMes)
                .SelectMany(m => m.Items)
                .Sum(i => i.Subtotal);

            var stockBajo = productos.Count(p => p.Stock <= 5);

            var ordenesVencidas = mantenimientos.Count(m =>
                m.FechaEntrega.HasValue &&
                m.FechaEntrega.Value < now &&
                m.Estado != "Completado" &&
                m.Estado != "Cancelado");

            var porEstado = new PorEstadoDto
            {
                Pendiente = mantenimientos.Count(m => m.Estado == "Pendiente"),
                EnProceso = mantenimientos.Count(m => m.Estado == "En Proceso"),
                Completado = mantenimientos.Count(m => m.Estado == "Completado"),
                Cancelado = mantenimientos.Count(m => m.Estado == "Cancelado"),
            };

            // Órdenes últimos 7 días
            var diasSemana = new[] { "Dom", "Lun", "Mar", "Mié", "Jue", "Vie", "Sáb" };
            var ultimaSemana = new List<OrdenesDelDiaDto>();
            for (int i = 6; i >= 0; i--)
            {
                var dia = now.Date.AddDays(-i);
                var diaUtc = DateTime.SpecifyKind(dia, DateTimeKind.Utc);
                var siguiente = diaUtc.AddDays(1);
                ultimaSemana.Add(new OrdenesDelDiaDto
                {
                    Dia = diasSemana[(int)dia.DayOfWeek],
                    Count = mantenimientos.Count(m => m.Fecha >= diaUtc && m.Fecha < siguiente)
                });
            }

            return new DashboardStatsDto
            {
                TotalClientes = totalClientes,
                TotalVehiculos = totalVehiculos,
                TotalProductos = productos.Count,
                TotalMantenimientos = mantenimientos.Count,
                OrdenesMes = ordenesMes,
                OrdenesHoy = ordenesHoy,
                IngresosMes = ingresosMes,
                StockBajo = stockBajo,
                MantenimientosPendientes = porEstado.Pendiente,
                OrdenesVencidas = ordenesVencidas,
                PorEstado = porEstado,
                UltimaSemana = ultimaSemana
            };
        }

        public async Task<List<MecanicoStatsDto>> GetMecanicoStatsAsync(string periodo = "mes")
        {
            var now = DateTime.UtcNow;
            var inicio = periodo switch
            {
                "semana" => now.AddDays(-7),
                "anio"   => new DateTime(now.Year, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                "todo"   => DateTime.MinValue,
                _        => new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc)
            };

            var mecanicos = await _userManager.GetUsersInRoleAsync("Mecanico");

            var ordenes = await _context.Mantenimientos
                .Include(m => m.Items)
                .Where(m => m.MecanicoId != null && m.Fecha >= inicio)
                .ToListAsync();

            var result = new List<MecanicoStatsDto>();
            foreach (var mec in mecanicos)
            {
                var propias = ordenes.Where(m => m.MecanicoId == mec.Id).ToList();
                result.Add(new MecanicoStatsDto
                {
                    MecanicoId = mec.Id.ToString(),
                    MecanicoNombre = mec.Nombre,
                    TotalOrdenes = propias.Count,
                    Completadas = propias.Count(m => m.Estado == "Completado"),
                    EnProceso = propias.Count(m => m.Estado == "En Proceso"),
                    Ingresos = propias
                        .Where(m => m.Estado == "Completado")
                        .SelectMany(m => m.Items)
                        .Sum(i => i.Subtotal)
                });
            }

            return result.OrderByDescending(m => m.TotalOrdenes).ToList();
        }
    }
}

