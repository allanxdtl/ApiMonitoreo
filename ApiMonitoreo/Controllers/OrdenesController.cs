using ApiMonitoreo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiMonitoreo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdenController : ControllerBase
    {
        private readonly MonitoreoContext _context;

        public OrdenController(MonitoreoContext monitoreo)
        {
            _context = monitoreo;
            QuestPDF.Settings.License = LicenseType.Community;
        }

        [HttpPost("CrearOrden")]
        public async Task<IActionResult> CrearOrden(int idCliente, int idProducto, int cantidad)
        {
            if (idCliente <= 0 || idProducto <= 0 || cantidad <= 0)
                return BadRequest("Datos inválidos.");

            var producto = await _context.ProductoTerminados
                .FirstOrDefaultAsync(p => p.ProductoId == idProducto);
            if (producto == null)
                return NotFound("Producto no encontrado.");

            // ✅ Ya no se valida existencia, se crea con estatus "Pendiente para producción"
            var nuevaOrden = new Orden
            {
                Idcliente = idCliente,
                Idproducto = idProducto,
                Cantidad = cantidad,
                FechaOrden = DateTime.Now,
                Estatus = "Pendiente para producción",
                Precio = producto.Precio*cantidad
            };

            _context.Ordens.Add(nuevaOrden);
            await _context.SaveChangesAsync();

            var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.Id == idCliente);

            try
            {
                var pdfBytes = GenerarPdfOrden(nuevaOrden, producto, cliente);
                return File(pdfBytes, "application/pdf", $"Orden_{nuevaOrden.Idorden}.pdf");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al generar el PDF: {ex.Message}");
            }
        }

        [HttpGet("OrdenesPendientes")]
        public async Task<IActionResult> Get()
        {
            var ordenes = await _context.Ordens
                                .Include(o => o.IdclienteNavigation)
                                .Include(o => o.IdproductoNavigation)
                                .Select(o => new
                                {
                                    o.Idorden,
                                    o.FechaOrden,
                                    o.IdproductoNavigation.ProductoId,
                                    o.IdproductoNavigation.Nombre,
                                    o.Cantidad,
                                    o.Estatus
                                })
                                .Where(o => o.Estatus == "Pendiente para producción")
                                .ToListAsync();

            return Ok(ordenes);
        }

        private byte[] GenerarPdfOrden(Orden orden, ProductoTerminado producto, Cliente cliente)
        {
            var fechaImpresion = DateTime.Now.ToString("dd/MM/yyyy HH:mm");

            var pdf = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(40);

                    page.Header().Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text("MONITOREO Y ADQUISICION DE DATOS").FontSize(18).Bold().FontColor(Colors.Blue.Medium);
                            col.Item().Text("Reporte de Orden de Producción").FontSize(13).Italic().FontColor(Colors.Grey.Darken1);
                            col.Item().Text($"Fecha de emisión: {fechaImpresion}").FontSize(9).FontColor(Colors.Grey.Darken2);
                        });
                    });

                    page.Content().PaddingVertical(20).Column(column =>
                    {
                        column.Item().BorderBottom(1).BorderColor(Colors.Grey.Lighten1)
                            .PaddingBottom(5)
                            .Text($"Orden #{orden.Idorden}")
                            .FontSize(14).Bold();

                        column.Item().PaddingVertical(10).Column(info =>
                        {
                            info.Item().Text($"Cliente: {cliente?.RazonSocial ?? "N/A"}").FontSize(11);
                            info.Item().Text($"Fecha de Pedido: {orden.FechaOrden:dd/MM/yyyy}").FontSize(10).FontColor(Colors.Grey.Darken1);
                        });

                        column.Item().PaddingTop(15).Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(250); // Producto
                                columns.ConstantColumn(100); // ID Producto
                                columns.ConstantColumn(100); // Cantidad
                                columns.RelativeColumn();   // Notas
                            });

                            table.Header(header =>
                            {
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Producto").Bold();
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("ID Producto").Bold();
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Cantidad").Bold();
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Notas").Bold();
                            });

                            // Fila de datos
                            table.Cell().Padding(5).Text(producto?.Nombre ?? "N/A");
                            table.Cell().Padding(5).Text(orden.Idproducto.ToString());
                            table.Cell().Padding(5).Text(orden.Cantidad.ToString("N2"));
                        });

                        column.Item().PaddingTop(20).AlignRight().Text(text =>
                        {
                            text.Span("Total de productos: ").FontSize(10).Bold();
                            text.Span("1").FontSize(10);
                        });

                        column.Item().PaddingTop(5).AlignRight().Text(text =>
                        {
                            text.Span("Cantidad total pedida: ").FontSize(10).Bold();
                            text.Span($"{orden.Cantidad:N2} unidades").FontSize(10);
                        });

						column.Item().PaddingTop(5).AlignRight().Text(text =>
						{
							text.Span("Total de la orden: ").FontSize(10).Bold();
							text.Span($"${orden.Precio:N2}").FontSize(10);
						});

						column.Item().PaddingTop(30)
                            .BorderTop(1)
                            .BorderColor(Colors.Grey.Lighten1)
                            .Text("Firma de autorización: __________________________")
                            .FontSize(10);
                    });

                    page.Footer().AlignCenter().Text(text =>
                    {
                        text.Span("Reporte generado automáticamente el ").FontColor(Colors.Grey.Darken2);
                        text.Span(fechaImpresion).FontColor(Colors.Blue.Medium);
                    });
                });
            });

            return pdf.GeneratePdf();
        }
    }
}
