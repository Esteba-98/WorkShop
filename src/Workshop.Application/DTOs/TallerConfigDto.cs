namespace Workshop.Application.DTOs.TallerConfig
{
    public class TallerConfigDto
    {
        public string Nombre { get; set; } = "WorkShop Taller Mecánico";
        public string Telefono { get; set; } = "";
        public string? Direccion { get; set; }
        public string? Email { get; set; }
        public string? NIT { get; set; }
    }

    public class UpdateTallerConfigDto
    {
        public string Nombre { get; set; } = "";
        public string Telefono { get; set; } = "";
        public string? Direccion { get; set; }
        public string? Email { get; set; }
        public string? NIT { get; set; }
    }
}
