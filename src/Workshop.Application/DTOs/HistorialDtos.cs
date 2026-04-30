namespace Workshop.Application.DTOs.Historial
{
    public class OrdenResumenDto
    {
        public Guid Id { get; set; }
        public string Folio { get; set; } = "";
        public DateTime Fecha { get; set; }
        public DateTime? FechaEntrega { get; set; }
        public string Estado { get; set; } = "";
        public string Descripcion { get; set; } = "";
        public int TotalItems { get; set; }
        public decimal Total { get; set; }
    }

    public class VehiculoHistorialDto
    {
        public Guid Id { get; set; }
        public string Placa { get; set; } = "";
        public string Marca { get; set; } = "";
        public string Modelo { get; set; } = "";
        public int Anio { get; set; }
        public string ClienteNombre { get; set; } = "";
        public string ClienteTelefono { get; set; } = "";
        public int TotalOrdenes { get; set; }
        public decimal TotalFacturado { get; set; }
        public List<OrdenResumenDto> Ordenes { get; set; } = new();
    }

    public class VehiculoEnHistorialDto
    {
        public Guid Id { get; set; }
        public string Placa { get; set; } = "";
        public string Marca { get; set; } = "";
        public string Modelo { get; set; } = "";
        public int Anio { get; set; }
        public int TotalOrdenes { get; set; }
        public List<OrdenResumenDto> Ordenes { get; set; } = new();
    }

    public class ClienteHistorialDto
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; } = "";
        public string Email { get; set; } = "";
        public string Telefono { get; set; } = "";
        public int TotalOrdenes { get; set; }
        public decimal TotalFacturado { get; set; }
        public List<VehiculoEnHistorialDto> Vehiculos { get; set; } = new();
    }
}
