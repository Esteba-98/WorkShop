namespace Workshop.Application.DTOs.Clientes
{
    public class ClienteDto
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; } = "";
        public string Email { get; set; } = "";
        public string Telefono { get; set; } = "";
    }

    public class CreateClienteDto
    {
        public string Nombre { get; set; } = "";
        public string Email { get; set; } = "";
        public string Telefono { get; set; } = "";
    }

    public class UpdateClienteDto
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; } = "";
        public string Email { get; set; } = "";
        public string Telefono { get; set; } = "";
    }
}
