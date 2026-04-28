using ClosedXML.Excel;
using Workshop.Domain.Entities;
using Workshop.Infrastructure.Services;
using Workshop.Tests.Helpers;

namespace Workshop.Tests
{
    public class ImportarExcelTests
    {
        /// <summary>
        /// Construye un .xlsx en memoria con las filas dadas.
        /// Cada fila es (Codigo, Nombre, Precio, Stock).
        /// </summary>
        private static Stream CrearExcel(params (string codigo, string nombre, string precio, string stock)[] filas)
        {
            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Productos");

            // Encabezado (igual que la plantilla real)
            ws.Cell(1, 1).Value = "Codigo";
            ws.Cell(1, 2).Value = "Nombre";
            ws.Cell(1, 3).Value = "Precio";
            ws.Cell(1, 4).Value = "Stock";

            for (int i = 0; i < filas.Length; i++)
            {
                int row = i + 2;
                ws.Cell(row, 1).Value = filas[i].codigo;
                ws.Cell(row, 2).Value = filas[i].nombre;
                // Intentar guardar como número cuando sea posible
                if (decimal.TryParse(filas[i].precio, out var precio))
                    ws.Cell(row, 3).Value = precio;
                else
                    ws.Cell(row, 3).Value = filas[i].precio;

                if (int.TryParse(filas[i].stock, out var stock))
                    ws.Cell(row, 4).Value = stock;
                else
                    ws.Cell(row, 4).Value = filas[i].stock;
            }

            var ms = new MemoryStream();
            wb.SaveAs(ms);
            ms.Position = 0;
            return ms;
        }

        // ── Creación ──────────────────────────────────────────────────────────

        [Fact]
        public async Task FilasNuevas_DebenCrearProductos()
        {
            var ctx = DbContextFactory.Create(nameof(FilasNuevas_DebenCrearProductos));
            var svc = new ProductoService(ctx);

            var stream = CrearExcel(
                ("REP-001", "Filtro aceite", "25000", "10"),
                ("REP-002", "Pastillas freno", "85000", "5")
            );

            var resultado = await svc.ImportarAsync(stream);

            Assert.Equal(2, resultado.Creados);
            Assert.Equal(0, resultado.Actualizados);
            Assert.Empty(resultado.Errores);
            Assert.Equal(2, ctx.Productos.Count());
        }

        // ── Actualización ─────────────────────────────────────────────────────

        [Fact]
        public async Task CodigoExistente_DebeActualizarPrecioYStock()
        {
            var ctx = DbContextFactory.Create(nameof(CodigoExistente_DebeActualizarPrecioYStock));
            ctx.Productos.Add(new Producto
            {
                Id = Guid.NewGuid(),
                Codigo = "REP-001",
                Nombre = "Filtro viejo",
                Precio = 20000,
                Stock = 3
            });
            await ctx.SaveChangesAsync();

            var svc = new ProductoService(ctx);
            var stream = CrearExcel(("REP-001", "Filtro actualizado", "30000", "15"));

            var resultado = await svc.ImportarAsync(stream);

            Assert.Equal(1, resultado.Actualizados);
            Assert.Equal(0, resultado.Creados);

            var producto = ctx.Productos.First();
            Assert.Equal(30000, producto.Precio);
            Assert.Equal(15, producto.Stock);
        }

        [Fact]
        public async Task CodigoExistente_NombreVacio_MantieneNombreOriginal()
        {
            var ctx = DbContextFactory.Create(nameof(CodigoExistente_NombreVacio_MantieneNombreOriginal));
            ctx.Productos.Add(new Producto
            {
                Id = Guid.NewGuid(),
                Codigo = "REP-001",
                Nombre = "Nombre original",
                Precio = 20000,
                Stock = 5
            });
            await ctx.SaveChangesAsync();

            var svc = new ProductoService(ctx);
            var stream = CrearExcel(("REP-001", "", "35000", "8"));

            await svc.ImportarAsync(stream);

            var producto = ctx.Productos.First();
            Assert.Equal("Nombre original", producto.Nombre); // no se borró
        }

        // ── Errores por fila ──────────────────────────────────────────────────

        [Fact]
        public async Task CodigoVacio_GeneraError_YNoCreaNada()
        {
            var ctx = DbContextFactory.Create(nameof(CodigoVacio_GeneraError_YNoCreaNada));
            var svc = new ProductoService(ctx);

            var stream = CrearExcel(("", "Sin código", "10000", "5"));

            var resultado = await svc.ImportarAsync(stream);

            Assert.Equal(0, resultado.Creados);
            Assert.NotEmpty(resultado.Errores);
        }

        [Fact]
        public async Task PrecioInvalido_GeneraError_OtrasFilasSiSeProcesan()
        {
            var ctx = DbContextFactory.Create(nameof(PrecioInvalido_GeneraError_OtrasFilasSiSeProcesan));
            var svc = new ProductoService(ctx);

            var stream = CrearExcel(
                ("REP-001", "Producto válido", "25000", "10"),
                ("REP-002", "Producto malo", "ABCDE", "5"),  // precio inválido
                ("REP-003", "Otro válido", "15000", "8")
            );

            var resultado = await svc.ImportarAsync(stream);

            Assert.Equal(2, resultado.Creados);   // REP-001 y REP-003
            Assert.Equal(1, resultado.Errores.Count); // REP-002
        }

        [Fact]
        public async Task ProductoNuevoSinNombre_GeneraError()
        {
            var ctx = DbContextFactory.Create(nameof(ProductoNuevoSinNombre_GeneraError));
            var svc = new ProductoService(ctx);

            var stream = CrearExcel(("REP-NEW", "", "10000", "5"));

            var resultado = await svc.ImportarAsync(stream);

            Assert.Equal(0, resultado.Creados);
            Assert.NotEmpty(resultado.Errores);
        }

        // ── Plantilla ─────────────────────────────────────────────────────────

        [Fact]
        public async Task GenerarPlantilla_DevuelveArchivoExcelValido()
        {
            var ctx = DbContextFactory.Create(nameof(GenerarPlantilla_DevuelveArchivoExcelValido));
            var svc = new ProductoService(ctx);

            var bytes = await svc.GenerarPlantillaAsync();

            Assert.NotNull(bytes);
            Assert.True(bytes.Length > 0);

            // Verificar que se puede abrir como Excel válido
            using var ms = new MemoryStream(bytes);
            var wb = new XLWorkbook(ms);
            var ws = wb.Worksheet(1);

            Assert.Equal("Codigo", ws.Cell(1, 1).GetValue<string>());
            Assert.Equal("Nombre", ws.Cell(1, 2).GetValue<string>());
            Assert.Equal("Precio", ws.Cell(1, 3).GetValue<string>());
            Assert.Equal("Stock", ws.Cell(1, 4).GetValue<string>());
        }
    }
}
