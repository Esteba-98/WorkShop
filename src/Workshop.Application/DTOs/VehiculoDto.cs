namespace Workshop.Application.DTOs.Vehiculos
{
    public class VehiculoDto
    {
        public Guid Id { get; set; }
        public string Placa { get; set; } = "";
        public string Marca { get; set; } = "";
        public string Modelo { get; set; } = "";
        public int Anio { get; set; }
        public Guid ClienteId { get; set; }
    }

    public class CreateVehiculoDto
    {
        public string Placa { get; set; } = "";
        public string Marca { get; set; } = "";
        public string Modelo { get; set; } = "";
        public int Anio { get; set; }
        public Guid ClienteId { get; set; }
    }

    public class UpdateVehiculoDto
    {
        public Guid Id { get; set; }
        public string Placa { get; set; } = "";
        public string Marca { get; set; } = "";
        public string Modelo { get; set; } = "";
        public int Anio { get; set; }
        public Guid ClienteId { get; set; }
    }
}
