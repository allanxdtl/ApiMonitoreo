using ApiMonitoreo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace ApiMonitoreo.Controllers
{
    public class BOMDetalle
    {
        public int IdMateriaPrima { get; set; }
        public string NombreMateriaPrima { get; set; }
        public decimal CantidadNecesaria { get; set; }
        public decimal ExistenciaActualMP { get; set; }
    }


    [ApiController]
    [Route("api/[controller]")]
    public class BOMController : ControllerBase
    {
        private readonly MonitoreoContext _context;

        public BOMController(MonitoreoContext context)
        {
            _context = context;
            QuestPDF.Settings.License = LicenseType.Community;
        }


        /// <summary>
        /// Inserta una materia prima necesaria para la producción del producto terminado
        /// </summary>
        /// <param name="bom">Objeto con los datos de la receta</param>
        /// <returns>Mensaje de inserción exitosa</returns>
        /// <remarks>
        /// Ejemplo de petición:
        ///
        /// POST /Insert
        /// 
        /// <code>
        /// {
        ///   "productoId": 1,
        ///   "materiaPrimaId": 2,
        ///   "cantidadNecesaria": 5
        /// }
        /// </code>
        /// </remarks>
        /// <response code="200">Regresa mensaje de inserción exitosa</response>
        /// <response code="400">Regresa mensaje de que los datos son inválidos</response>
        [HttpPost("Insert")]
        public async Task<IActionResult> Insert([FromBody] RecetaProducto bom)
        {
            if (bom == null || bom.ProductoId <= 0 || bom.MateriaPrimaId <= 0 || bom.CantidadNecesaria <= 0)
                return BadRequest("Datos inválidos");

            _context.RecetaProductos.Add(bom);
            await _context.SaveChangesAsync();
            return Ok("BOM agregado correctamente");
        }

        // Consultar el BOM de un producto
        [HttpGet("GetByProducto/{productoId}")]
        public async Task<IActionResult> GetByProducto(int productoId)
        {
            // Obtener el producto terminado
            var producto = _context.ProductoTerminados.FirstOrDefault(p => p.ProductoId == productoId);
            if (producto == null) return NotFound("Producto no encontrado");

            // Obtener detalles del BOM
            var detalles = await (from b in _context.RecetaProductos
                                  join mp in _context.MateriaPrimas on b.MateriaPrimaId equals mp.MateriaPrimaId
                                  where b.ProductoId == productoId
                                  select new BOMDetalle
                                  {
                                      IdMateriaPrima = mp.MateriaPrimaId,
                                      NombreMateriaPrima = mp.Nombre,
                                      CantidadNecesaria = b.CantidadNecesaria,
                                      ExistenciaActualMP = _context.LoteMateriaPrimas
                                          .Where(l => l.MateriaPrimaId == mp.MateriaPrimaId)
                                          .Sum(l => (decimal?)l.CantidadDisponible) ?? 0
                                  }).ToListAsync();

            // Existencia actual del producto terminado
            decimal existenciaProducto = _context.ProductoTerminados
                .Where(p => p.ProductoId == productoId)
                .Select(p => p.ExistenciaActual)
                .FirstOrDefault();

            // Generar PDF
            var pdfBytes = GenerarPdfBOM(producto.ProductoId, producto.Nombre, detalles, existenciaProducto);

            // Retornar archivo para descarga
            return File(pdfBytes, "application/pdf", $"BOM_{producto.ProductoId}.pdf");
        }

        private byte[] GenerarPdfBOM(int productoId, string nombreProducto, List<BOMDetalle> detalles, decimal existenciaProducto)
        {
            var fechaImpresion = DateTime.Now.ToString("dd/MM/yyyy HH:mm");

            var pdf = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(40);

                    // ==== ENCABEZADO ====
                    page.Header().Row(row =>
                    {

                        // Columna derecha: Títulos
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text("Reporte de Materiales (BOM)")
                                .FontSize(18)
                                .Bold()
                                .FontColor("#2c3e50");

                            col.Item().Text($"Producto terminado: {nombreProducto}")
                                .FontSize(12)
                                .FontColor("#34495e");

                            col.Item().Text($"ID Producto: {productoId}")
                                .FontSize(11)
                                .FontColor("#7f8c8d");

                            col.Item().Text($"Fecha de emisión: {fechaImpresion}")
                                .FontSize(10)
                                .Italic()
                                .FontColor("#95a5a6");
                        });
                    });

                    page.Content().PaddingVertical(15).Column(col =>
                    {
                        // ==== Subtítulo ====
                        col.Item()
                            .PaddingBottom(5)
                            .Text("Detalles de materiales requeridos")
                            .FontSize(14)
                            .Bold()
                            .FontColor("#2c3e50");

                        // ==== TABLA ====
                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(60);   // ID
                                columns.RelativeColumn(3);    // Nombre MP
                                columns.RelativeColumn(2);    // Cantidad Necesaria
                                columns.RelativeColumn(2);    // Existencia Actual
                            });

                            // === Encabezado ===
                            table.Header(header =>
                            {
                                header.Cell().Element(CellStyleHeader).Text("ID").FontColor("#ffffff").Bold();
                                header.Cell().Element(CellStyleHeader).Text("Materia Prima").FontColor("#ffffff").Bold();
                                header.Cell().Element(CellStyleHeader).Text("Cantidad Necesaria").FontColor("#ffffff").Bold();
                                header.Cell().Element(CellStyleHeader).Text("Existencia Actual MP").FontColor("#ffffff").Bold();
                            });

                            // === Filas dinámicas ===
                            bool alternate = false;
                            foreach (var d in detalles)
                            {
                                string bgColor = alternate ? "#f8f9fa" : "#ffffff";
                                alternate = !alternate;

                                table.Cell().Element(c => CellStyleRow(c, bgColor)).Text(d.IdMateriaPrima.ToString());
                                table.Cell().Element(c => CellStyleRow(c, bgColor)).Text(d.NombreMateriaPrima);
                                table.Cell().Element(c => CellStyleRow(c, bgColor)).AlignRight().Text($"{d.CantidadNecesaria:N2}");
                                table.Cell().Element(c => CellStyleRow(c, bgColor)).AlignRight().Text($"{d.ExistenciaActualMP:N2}");
                            }

                            // === Totales (opcional, si aplica) ===
                            table.Cell().ColumnSpan(4).PaddingTop(5)
                                .BorderTop(1)
                                .AlignRight()
                                .Text($"Total de materiales: {detalles.Count}")
                                .FontSize(11)
                                .Bold()
                                .FontColor("#2c3e50");
                        });
                    });

                    // ==== PIE DE PÁGINA ====
                    page.Footer().AlignRight().Text(txt =>
                    {
                        txt.Span("Existencia actual del producto terminado: ")
                           .Bold()
                           .FontColor("#2c3e50");
                        txt.Span($"{existenciaProducto:N2} unidades")
                           .FontColor("#27ae60");
                    });
                });
            });

            return pdf.GeneratePdf();

            // ==== FUNCIONES DE ESTILO ====
            static IContainer CellStyleHeader(IContainer container)
            {
                return container
                    .Background("#2c3e50")
                    .PaddingVertical(6)
                    .PaddingHorizontal(4)
                    .BorderBottom(1)
                    .BorderColor("#34495e");
            }

            static IContainer CellStyleRow(IContainer container, string background)
            {
                return container
                    .Background(background)
                    .PaddingVertical(5)
                    .PaddingHorizontal(4)
                    .BorderBottom(0.5f)
                    .BorderColor("#ecf0f1");
            }
        }
    }
}
