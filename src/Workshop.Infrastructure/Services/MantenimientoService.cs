using ClosedXML.Excel;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Workshop.Application.DTOs.Mantenimientos;
using Workshop.Application.Services;
using Workshop.Domain.Entities;
using Workshop.Infrastructure.Persistence;

namespace Workshop.Infrastructure.Services
{
    public class MantenimientoService : IMantenimientoService
    {
        private readonly WorkshopDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public MantenimientoService(WorkshopDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        private async Task<string?> GetMecanicoNombre(Guid? mecanicoId)
        {
            if (mecanicoId == null) return null;
            var user = await _userManager.FindByIdAsync(mecanicoId.ToString()!);
            return user?.Nombre;
        }

        private static MantenimientoItemDto MapItem(MantenimientoItem i) => new()
        {
            Id = i.Id,
            Tipo = i.Tipo,
            Nombre = i.Nombre,
            ProductoId = i.ProductoId,
            Cantidad = i.Cantidad,
            PrecioUnitario = i.PrecioUnitario,
            Subtotal = i.Subtotal
        };

        private async Task<MantenimientoDto> MapDto(Mantenimiento m) => new()
        {
            Id = m.Id,
            Folio = m.Folio,
            ClienteId = m.ClienteId,
            ClienteNombre = m.Cliente?.Nombre ?? "",
            ClienteTelefono = m.Cliente?.Telefono ?? "",
            VehiculoId = m.VehiculoId,
            VehiculoPlaca = m.Vehiculo?.Placa ?? "",
            VehiculoMarca = m.Vehiculo?.Marca ?? "",
            VehiculoModelo = m.Vehiculo?.Modelo ?? "",
            VehiculoAnio = m.Vehiculo?.Anio ?? 0,
            MecanicoId = m.MecanicoId,
            MecanicoNombre = await GetMecanicoNombre(m.MecanicoId),
            Fecha = m.Fecha,
            FechaEntrega = m.FechaEntrega,
            Estado = m.Estado,
            Pagado = m.Pagado,
            Descripcion = m.Descripcion,
            Diagnostico = m.Diagnostico,
            Observaciones = m.Observaciones,
            Items = m.Items.Select(MapItem).ToList()
        };

        private IQueryable<Mantenimiento> QueryConRelaciones() =>
            _context.Mantenimientos
                .Include(m => m.Cliente)
                .Include(m => m.Vehiculo)
                .Include(m => m.Items).ThenInclude(i => i.Producto);

        // Genera el próximo folio: OT-0001
        private async Task<string> GenerarFolioAsync()
        {
            var ultimo = await _context.Mantenimientos
                .OrderByDescending(m => m.Folio)
                .Select(m => m.Folio)
                .FirstOrDefaultAsync();

            int numero = 1;
            if (!string.IsNullOrEmpty(ultimo) && ultimo.StartsWith("OT-"))
            {
                if (int.TryParse(ultimo[3..], out int n)) numero = n + 1;
            }
            return $"OT-{numero:D4}";
        }

        public async Task<List<MantenimientoDto>> GetAllAsync()
        {
            var lista = await QueryConRelaciones().ToListAsync();
            var result = new List<MantenimientoDto>();
            foreach (var m in lista)
                result.Add(await MapDto(m));
            return result;
        }

        public async Task<MantenimientoDto?> GetByIdAsync(Guid id)
        {
            var m = await QueryConRelaciones().FirstOrDefaultAsync(x => x.Id == id);
            if (m == null) return null;
            return await MapDto(m);
        }

        public async Task<MantenimientoDto> CreateAsync(CreateMantenimientoDto dto)
        {
            var mantenimiento = new Mantenimiento
            {
                Id = Guid.NewGuid(),
                Folio = await GenerarFolioAsync(),
                VehiculoId = dto.VehiculoId,
                ClienteId = dto.ClienteId,
                MecanicoId = dto.MecanicoId,
                Descripcion = dto.Descripcion,
                FechaEntrega = dto.FechaEntrega.HasValue
                    ? DateTime.SpecifyKind(dto.FechaEntrega.Value, DateTimeKind.Utc)
                    : null,
                Fecha = DateTime.UtcNow,
                Estado = "Pendiente"
            };

            _context.Mantenimientos.Add(mantenimiento);
            await _context.SaveChangesAsync();

            var creado = await QueryConRelaciones().FirstAsync(x => x.Id == mantenimiento.Id);
            return await MapDto(creado);
        }

        public async Task<MantenimientoDto?> UpdateAsync(UpdateMantenimientoDto dto)
        {
            var mantenimiento = await QueryConRelaciones().FirstOrDefaultAsync(x => x.Id == dto.Id);
            if (mantenimiento == null) return null;

            mantenimiento.Estado = dto.Estado;
            mantenimiento.VehiculoId = dto.VehiculoId;
            mantenimiento.ClienteId = dto.ClienteId;
            mantenimiento.MecanicoId = dto.MecanicoId;
            mantenimiento.Descripcion = dto.Descripcion;
            mantenimiento.Diagnostico = dto.Diagnostico;
            mantenimiento.Observaciones = dto.Observaciones;
            mantenimiento.Fecha = DateTime.SpecifyKind(dto.Fecha, DateTimeKind.Utc);
            mantenimiento.FechaEntrega = dto.FechaEntrega.HasValue
                ? DateTime.SpecifyKind(dto.FechaEntrega.Value, DateTimeKind.Utc)
                : null;

            await _context.SaveChangesAsync();
            return await MapDto(mantenimiento);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var mantenimiento = await _context.Mantenimientos.FindAsync(id);
            if (mantenimiento == null) return false;
            _context.Mantenimientos.Remove(mantenimiento);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<MantenimientoDto?> AddItemAsync(Guid mantenimientoId, AddItemDto dto)
        {
            var mantenimiento = await QueryConRelaciones().FirstOrDefaultAsync(x => x.Id == mantenimientoId);
            if (mantenimiento == null) return null;

            string nombre = dto.Nombre;
            decimal precio = dto.PrecioUnitario;

            // Si es producto del almacén: validar stock y tomar precio
            if (dto.Tipo == "Producto" && dto.ProductoId.HasValue)
            {
                var producto = await _context.Productos.FindAsync(dto.ProductoId.Value);
                if (producto == null) return null;
                if (producto.Stock < dto.Cantidad) return null; // stock insuficiente

                // Descontar stock
                producto.Stock -= dto.Cantidad;
                nombre = producto.Nombre;
                precio = producto.Precio;
            }

            var item = new MantenimientoItem
            {
                Id = Guid.NewGuid(),
                MantenimientoId = mantenimientoId,
                Tipo = dto.Tipo,
                Nombre = nombre,
                ProductoId = dto.ProductoId,
                Cantidad = dto.Cantidad,
                PrecioUnitario = precio
            };

            _context.MantenimientoItems.Add(item);
            await _context.SaveChangesAsync();

            var actualizado = await QueryConRelaciones().FirstAsync(x => x.Id == mantenimientoId);
            return await MapDto(actualizado);
        }

        public async Task<MantenimientoDto?> TogglePagadoAsync(Guid id)
        {
            var mantenimiento = await QueryConRelaciones().FirstOrDefaultAsync(x => x.Id == id);
            if (mantenimiento == null) return null;
            mantenimiento.Pagado = !mantenimiento.Pagado;
            await _context.SaveChangesAsync();
            return await MapDto(mantenimiento);
        }

        public async Task<byte[]> ExportExcelAsync(DateTime? desde, DateTime? hasta)
        {
            var query = QueryConRelaciones();
            if (desde.HasValue)
                query = query.Where(m => m.Fecha >= DateTime.SpecifyKind(desde.Value, DateTimeKind.Utc));
            if (hasta.HasValue)
                query = query.Where(m => m.Fecha <= DateTime.SpecifyKind(hasta.Value.AddDays(1), DateTimeKind.Utc));

            var lista = await query.OrderByDescending(m => m.Fecha).ToListAsync();

            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Órdenes");

            string[] headers = { "Folio", "Fecha", "Estado", "Pagado", "Cliente", "Teléfono", "Vehículo", "Placa", "Mecánico", "Total" };
            for (int i = 0; i < headers.Length; i++)
            {
                var cell = ws.Cell(1, i + 1);
                cell.Value = headers[i];
                cell.Style.Font.Bold = true;
                cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#F97316");
                cell.Style.Font.FontColor = XLColor.White;
            }

            int row = 2;
            foreach (var m in lista)
            {
                var dto = await MapDto(m);
                ws.Cell(row, 1).Value = dto.Folio;
                ws.Cell(row, 2).Value = dto.Fecha.ToString("dd/MM/yyyy");
                ws.Cell(row, 3).Value = dto.Estado;
                ws.Cell(row, 4).Value = dto.Pagado ? "Sí" : "No";
                ws.Cell(row, 5).Value = dto.ClienteNombre;
                ws.Cell(row, 6).Value = dto.ClienteTelefono;
                ws.Cell(row, 7).Value = $"{dto.VehiculoMarca} {dto.VehiculoModelo} {dto.VehiculoAnio}";
                ws.Cell(row, 8).Value = dto.VehiculoPlaca;
                ws.Cell(row, 9).Value = dto.MecanicoNombre ?? "";
                ws.Cell(row, 10).Value = (double)dto.Total;
                ws.Cell(row, 10).Style.NumberFormat.Format = "#,##0";
                row++;
            }

            ws.Columns().AdjustToContents();

            using var ms = new MemoryStream();
            workbook.SaveAs(ms);
            return ms.ToArray();
        }

        public async Task<MantenimientoDto?> RemoveItemAsync(Guid mantenimientoId, Guid itemId)
        {
            var item = await _context.MantenimientoItems
                .Include(i => i.Producto)
                .FirstOrDefaultAsync(i => i.Id == itemId && i.MantenimientoId == mantenimientoId);

            if (item == null) return null;

            // Devolver stock si era producto del almacén
            if (item.Tipo == "Producto" && item.ProductoId.HasValue)
            {
                var producto = await _context.Productos.FindAsync(item.ProductoId.Value);
                if (producto != null) producto.Stock += item.Cantidad;
            }

            _context.MantenimientoItems.Remove(item);
            await _context.SaveChangesAsync();

            var actualizado = await QueryConRelaciones().FirstAsync(x => x.Id == mantenimientoId);
            return await MapDto(actualizado);
        }
    }
}
