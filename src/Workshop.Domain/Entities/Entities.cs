namespace Workshop.Domain.Entities;

public class Cliente
{
    public Guid Id { get; set; }
    public string Nombre { get; set; } = "";
    public string Email { get; set; } = "";
    public string Telefono { get; set; } = "";
    public ICollection<Vehiculo> Vehiculos { get; set; } = new List<Vehiculo>();
}

public class Vehiculo
{
    public Guid Id { get; set; }
    public string Placa { get; set; } = "";
    public string Marca { get; set; } = "";
    public string Modelo { get; set; } = "";
    public int Anio { get; set; }

    public Guid ClienteId { get; set; }
    public Cliente Cliente { get; set; } = null!;

    public ICollection<Mantenimiento> Mantenimientos { get; set; } = new List<Mantenimiento>();
}

public class Producto
{
    public Guid Id { get; set; }
    public string Nombre { get; set; } = "";
    public string Codigo { get; set; } = "";
    public decimal Precio { get; set; }
    public int Stock { get; set; }

    public ICollection<MantenimientoItem> MantenimientoItems { get; set; } = new List<MantenimientoItem>();
}

public class Mantenimiento
{
    public Guid Id { get; set; }

    // Folio autoincremental: OT-0001, OT-0002...
    public string Folio { get; set; } = "";

    public Guid ClienteId { get; set; }
    public Cliente Cliente { get; set; } = null!;

    public Guid VehiculoId { get; set; }
    public Vehiculo Vehiculo { get; set; } = null!;

    public Guid? MecanicoId { get; set; }

    public DateTime Fecha { get; set; } = DateTime.UtcNow;
    public DateTime? FechaEntrega { get; set; }

    public string Estado { get; set; } = "Pendiente";

    // Descripción del trabajo solicitado por el cliente
    public string Descripcion { get; set; } = "";

    // Diagnóstico del mecánico
    public string Diagnostico { get; set; } = "";

    // Observaciones adicionales
    public string Observaciones { get; set; } = "";

    public ICollection<MantenimientoItem> Items { get; set; } = new List<MantenimientoItem>();
}

public class MantenimientoItem
{
    public Guid Id { get; set; }

    public Guid MantenimientoId { get; set; }
    public Mantenimiento Mantenimiento { get; set; } = null!;

    // Tipo: "Producto" o "Servicio"
    public string Tipo { get; set; } = "Producto";

    // Nombre del item (para servicios manuales o referencia del producto)
    public string Nombre { get; set; } = "";

    // Solo se asigna si el item es un producto del almacén
    public Guid? ProductoId { get; set; }
    public Producto? Producto { get; set; }

    public int Cantidad { get; set; } = 1;
    public decimal PrecioUnitario { get; set; }

    public decimal Subtotal => Cantidad * PrecioUnitario;
}
