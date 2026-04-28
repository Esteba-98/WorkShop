using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using Workshop.Application.DTOs.Productos;
using Workshop.Application.Services;
using Workshop.Domain.Entities;
using Workshop.Infrastructure.Persistence;


namespace Workshop.Infrastructure.Services
{
    public class ProductoService : IProductoService
    {
        private readonly WorkshopDbContext _context;

        public ProductoService(WorkshopDbContext context)
        {
            _context = context;
        }

        public async Task<List<ProductoDto>> GetAllAsync()
        {
            return await _context.Productos
                .Select(p => new ProductoDto
                {
                    Id = p.Id,
                    Nombre = p.Nombre,
                    Codigo = p.Codigo,
                    Precio = p.Precio,
                    Stock = p.Stock
                })
                .ToListAsync();
        }

        public async Task<ProductoDto?> GetByIdAsync(Guid id)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null) return null;

            return new ProductoDto
            {
                Id = producto.Id,
                Nombre = producto.Nombre,
                Codigo = producto.Codigo,
                Precio = producto.Precio,
                Stock = producto.Stock
            };
        }

        public async Task<ProductoDto> CreateAsync(CreateProductoDto dto)
        {
            var producto = new Producto
            {
                Id = Guid.NewGuid(),
                Nombre = dto.Nombre,
                Codigo = dto.Codigo,
                Precio = dto.Precio,
                Stock = dto.Stock
            };

            _context.Productos.Add(producto);
            await _context.SaveChangesAsync();

            return new ProductoDto
            {
                Id = producto.Id,
                Nombre = producto.Nombre,
                Codigo = producto.Codigo,
                Precio = producto.Precio,
                Stock = producto.Stock
            };
        }

        public async Task<ProductoDto?> UpdateAsync(UpdateProductoDto dto)
        {
            var producto = await _context.Productos.FindAsync(dto.Id);
            if (producto == null) return null;

            producto.Nombre = dto.Nombre;
            producto.Codigo = dto.Codigo;
            producto.Precio = dto.Precio;
            producto.Stock = dto.Stock;

            await _context.SaveChangesAsync();

            return new ProductoDto
            {
                Id = producto.Id,
                Nombre = producto.Nombre,
                Codigo = producto.Codigo,
                Precio = producto.Precio,
                Stock = producto.Stock
            };
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null) return false;

            _context.Productos.Remove(producto);
            await _context.SaveChangesAsync();
            return true;
        }

        public Task<byte[]> GenerarPlantillaAsync()
        {
            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Productos");

            // Encabezados
            ws.Cell(1, 1).Value = "Codigo";
            ws.Cell(1, 2).Value = "Nombre";
            ws.Cell(1, 3).Value = "Precio";
            ws.Cell(1, 4).Value = "Stock";

            var headerRow = ws.Range(1, 1, 1, 4);
            headerRow.Style.Font.Bold = true;
            headerRow.Style.Fill.BackgroundColor = XLColor.FromHtml("#1e293b"); // slate-800
            headerRow.Style.Font.FontColor = XLColor.White;
            headerRow.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            // Fila de ejemplo (en gris para que se note que es solo guía)
            ws.Cell(2, 1).Value = "REP-001";
            ws.Cell(2, 2).Value = "Filtro de aceite Honda";
            ws.Cell(2, 3).Value = 25000;
            ws.Cell(2, 4).Value = 10;
            var exampleRow = ws.Range(2, 1, 2, 4);
            exampleRow.Style.Font.FontColor = XLColor.FromHtml("#94a3b8"); // slate-400
            exampleRow.Style.Font.Italic = true;

            // Anchos de columna
            ws.Column(1).Width = 16;
            ws.Column(2).Width = 32;
            ws.Column(3).Width = 14;
            ws.Column(4).Width = 10;

            // Formato numérico en columnas Precio y Stock
            ws.Column(3).Style.NumberFormat.Format = "#,##0";
            ws.Column(4).Style.NumberFormat.Format = "0";

            using var ms = new MemoryStream();
            workbook.SaveAs(ms);
            return Task.FromResult(ms.ToArray());
        }

        public async Task<ImportarResultadoDto> ImportarAsync(Stream excelStream)
        {
            var resultado = new ImportarResultadoDto();

            using var workbook = new XLWorkbook(excelStream);
            var ws = workbook.Worksheet(1);

            int rowNum = 1;
            foreach (var row in ws.RowsUsed().Skip(1)) // saltar encabezado
            {
                rowNum++;
                try
                {
                    var codigo = row.Cell(1).GetValue<string>().Trim();

                    if (string.IsNullOrEmpty(codigo))
                    {
                        resultado.Errores.Add($"Fila {rowNum}: Código vacío, se omitió.");
                        continue;
                    }

                    var nombre = row.Cell(2).GetValue<string>().Trim();

                    if (!row.Cell(3).TryGetValue<decimal>(out var precio) || precio < 0)
                    {
                        resultado.Errores.Add($"Fila {rowNum} ({codigo}): Precio inválido.");
                        continue;
                    }

                    if (!row.Cell(4).TryGetValue<int>(out var stock) || stock < 0)
                    {
                        resultado.Errores.Add($"Fila {rowNum} ({codigo}): Stock inválido.");
                        continue;
                    }

                    var existente = await _context.Productos
                        .FirstOrDefaultAsync(p => p.Codigo == codigo);

                    if (existente != null)
                    {
                        if (!string.IsNullOrEmpty(nombre)) existente.Nombre = nombre;
                        existente.Precio = precio;
                        existente.Stock = stock;
                        resultado.Actualizados++;
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(nombre))
                        {
                            resultado.Errores.Add($"Fila {rowNum} ({codigo}): Nombre vacío para producto nuevo.");
                            continue;
                        }
                        _context.Productos.Add(new Producto
                        {
                            Id = Guid.NewGuid(),
                            Codigo = codigo,
                            Nombre = nombre,
                            Precio = precio,
                            Stock = stock
                        });
                        resultado.Creados++;
                    }
                }
                catch (Exception)
                {
                    resultado.Errores.Add($"Fila {rowNum}: Error inesperado al procesar.");
                }
            }

            await _context.SaveChangesAsync();
            return resultado;
        }
    }
}
