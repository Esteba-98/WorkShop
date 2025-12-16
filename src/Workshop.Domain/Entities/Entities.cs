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

    // 🔹 Relación inversa
    public ICollection<Mantenimiento> Mantenimientos { get; set; } = new List<Mantenimiento>();
}

public class Producto
{
    public Guid Id { get; set; }
    public string Nombre { get; set; } = "";
    public string Codigo { get; set; } = "";
    public decimal Precio { get; set; }
    public int Stock { get; set; }

    // 🔹 Relación inversa opcional
    public ICollection<MantenimientoItem> MantenimientoItems { get; set; } = new List<MantenimientoItem>();
}

public class Mantenimiento
{
    public Guid Id { get; set; }

    public Guid ClienteId { get; set; }
    public Cliente Cliente { get; set; } = null!;

    public Guid VehiculoId { get; set; }
    public Vehiculo Vehiculo { get; set; } = null!;

    public DateTime Fecha { get; set; } = DateTime.UtcNow;
    public string Estado { get; set; } = "Abierto";

    public ICollection<MantenimientoItem> Items { get; set; } = new List<MantenimientoItem>();
}

public class MantenimientoItem
{
    public Guid Id { get; set; }

    public Guid MantenimientoId { get; set; }
    public Mantenimiento Mantenimiento { get; set; } = null!;

    public Guid ProductoId { get; set; }
    public Producto Producto { get; set; } = null!;

    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }

    // 🔹 Campo calculado
    public decimal Subtotal => Cantidad * PrecioUnitario;
}
