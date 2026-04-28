namespace Workshop.Application.DTOs.Productos
{
    public class ProductoDto
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; } = "";
        public string Codigo { get; set; } = "";
        public decimal Precio { get; set; }
        public int Stock { get; set; }
    }

    public class CreateProductoDto
    {
        public string Nombre { get; set; } = "";
        public string Codigo { get; set; } = "";
        public decimal Precio { get; set; }
        public int Stock { get; set; }
    }

    public class UpdateProductoDto
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; } = "";
        public string Codigo { get; set; } = "";
        public decimal Precio { get; set; }
        public int Stock { get; set; }
    }

    public class ImportarResultadoDto
    {
        public int Actualizados { get; set; }
        public int Creados { get; set; }
        public List<string> Errores { get; set; } = new();
    }
}
