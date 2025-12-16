namespace Workshop.Application.DTOs.Mantenimientos
{
    public class MantenimientoDto
    {
        public Guid Id { get; set; }
        public Guid ClienteId { get; set; }
        public Guid VehiculoId { get; set; }
        public DateTime Fecha { get; set; }
        public string Estado { get; set; } = "";
    }

    public class CreateMantenimientoDto
    {
        public Guid ClienteId { get; set; }
        public Guid VehiculoId { get; set; }
    }

    public class UpdateMantenimientoDto
    {
        public Guid Id { get; set; }
        public Guid ClienteId { get; set; }
        public Guid VehiculoId { get; set; }
        public DateTime Fecha { get; set; }
        public string Estado { get; set; } = "";
    }
}
