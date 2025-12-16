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
    }
}
