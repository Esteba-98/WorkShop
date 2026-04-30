using Microsoft.EntityFrameworkCore;
using Workshop.Application.DTOs.Stats;
using Workshop.Application.Services;
using Workshop.Infrastructure.Persistence;

namespace Workshop.Infrastructure.Services
{
    public class StatsService : IStatsService
    {
        private readonly WorkshopDbContext _context;

        public StatsService(WorkshopDbContext context)
        {
            _context = context;
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
                PorEstado = porEstado,
                UltimaSemana = ultimaSemana
            };
        }
    }
}
