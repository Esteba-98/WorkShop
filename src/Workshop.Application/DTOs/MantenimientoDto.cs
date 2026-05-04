namespace Workshop.Application.DTOs.Mantenimientos
{
    public class MantenimientoItemDto
    {
        public Guid Id { get; set; }
        public string Tipo { get; set; } = "Producto"; // "Producto" | "Servicio"
        public string Nombre { get; set; } = "";
        public Guid? ProductoId { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal { get; set; }
    }

    public class MantenimientoDto
    {
        public Guid Id { get; set; }
        public string Folio { get; set; } = "";
        public Guid ClienteId { get; set; }
        public string ClienteNombre { get; set; } = "";
        public string ClienteTelefono { get; set; } = "";
        public Guid VehiculoId { get; set; }
        public string VehiculoPlaca { get; set; } = "";
        public string VehiculoMarca { get; set; } = "";
        public string VehiculoModelo { get; set; } = "";
        public int VehiculoAnio { get; set; }
        public Guid? MecanicoId { get; set; }
        public string? MecanicoNombre { get; set; }
        public DateTime Fecha { get; set; }
        public DateTime? FechaEntrega { get; set; }
        public string Estado { get; set; } = "";
        public bool Pagado { get; set; }
        public string Descripcion { get; set; } = "";
        public string Diagnostico { get; set; } = "";
        public string Observaciones { get; set; } = "";
        public List<MantenimientoItemDto> Items { get; set; } = new();
        public decimal Total => Items.Sum(i => i.Subtotal);
    }

    public class CreateMantenimientoDto
    {
        public Guid ClienteId { get; set; }
        public Guid VehiculoId { get; set; }
        public Guid? MecanicoId { get; set; }
        public string Descripcion { get; set; } = "";
        public DateTime? FechaEntrega { get; set; }
    }

    public class UpdateMantenimientoDto
    {
        public Guid Id { get; set; }
        public Guid ClienteId { get; set; }
        public Guid VehiculoId { get; set; }
        public Guid? MecanicoId { get; set; }
        public DateTime Fecha { get; set; }
        public DateTime? FechaEntrega { get; set; }
        public string Estado { get; set; } = "";
        public string Descripcion { get; set; } = "";
        public string Diagnostico { get; set; } = "";
        public string Observaciones { get; set; } = "";
    }

    // Agregar un item a la orden (producto del stock o servicio manual)
    public class AddItemDto
    {
        public string Tipo { get; set; } = "Producto"; // "Producto" | "Servicio"
        public string Nombre { get; set; } = "";       // Para servicios
        public Guid? ProductoId { get; set; }          // Para productos del stock
        public int Cantidad { get; set; } = 1;
        public decimal PrecioUnitario { get; set; }
    }
}
