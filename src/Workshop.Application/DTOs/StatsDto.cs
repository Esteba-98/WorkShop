namespace Workshop.Application.DTOs.Stats
{
    public class DashboardStatsDto
    {
        // Totales generales
        public int TotalClientes { get; set; }
        public int TotalVehiculos { get; set; }
        public int TotalProductos { get; set; }
        public int TotalMantenimientos { get; set; }

        // Stats del período actual
        public int OrdenesMes { get; set; }
        public int OrdenesHoy { get; set; }
        public decimal IngresosMes { get; set; }
        public int StockBajo { get; set; }
        public int MantenimientosPendientes { get; set; }

        // Distribución por estado
        public PorEstadoDto PorEstado { get; set; } = new();

        // Órdenes últimos 7 días
        public List<OrdenesDelDiaDto> UltimaSemana { get; set; } = new();
    }

    public class PorEstadoDto
    {
        public int Pendiente { get; set; }
        public int EnProceso { get; set; }
        public int Completado { get; set; }
        public int Cancelado { get; set; }
    }

    public class OrdenesDelDiaDto
    {
        public string Dia { get; set; } = "";
        public int Count { get; set; }
    }
}
