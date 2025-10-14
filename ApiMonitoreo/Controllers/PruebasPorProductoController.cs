using ApiMonitoreo.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace ApiMonitoreo.Controllers
{

    public class PruebaDto
    {
        public int Id { get; set; }
        public int ProductoId { get; set; }
        public int PruebaId { get; set; }
        public string Comentario { get; set; }
        public decimal ValorEsperado { get; set; }
        public decimal Tolerancia { get; set; }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class PruebasPorProductoController : ControllerBase
    {
        private readonly MonitoreoContext _context;
        public PruebasPorProductoController(MonitoreoContext context)
        {
            _context = context;
        }

        [HttpGet("ListPorProducto/{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var result = await _context.PruebaProductos
                .Where(p => p.ProductoId == id)
                .Select(p => new
                {
                    p.Idprueba,
                    p.Comentario,
                    p.ValorEsperado,
                    p.Tolerancia
                })
                .ToListAsync();

            return Ok(result);
        }

        [HttpPost("Insert")]
        public async Task<IActionResult> Insert([FromBody] PruebaDto pruebaProducto)
        {
            var pruebaModel = new PruebaProducto();
            pruebaModel.ProductoId = pruebaProducto.ProductoId;
            pruebaModel.Idprueba = pruebaProducto.PruebaId;
            pruebaModel.Comentario = pruebaProducto.Comentario;
            pruebaModel.ValorEsperado = pruebaProducto.ValorEsperado;
            pruebaModel.Tolerancia = pruebaProducto.Tolerancia;

            await _context.PruebaProductos.AddAsync(pruebaModel);
            await _context.SaveChangesAsync();

            var productoObj = await _context.PruebaProductos
                .Include(p => p.Producto)
                .FirstOrDefaultAsync(p => p.ProductoId == pruebaProducto.ProductoId);

            return Ok(new { message = $"Prueba insertada correctamente al producto {productoObj.Producto.Nombre}" });
        }

        [HttpPut("Edit")]
        public async Task<IActionResult> Edit([FromBody] PruebaDto pruebaProducto)
        {
            var existente = await _context.PruebaProductos.FindAsync(pruebaProducto.Id);

            if (existente == null)
                return NotFound();

            existente.Idprueba = pruebaProducto.PruebaId;
            existente.ProductoId = pruebaProducto.ProductoId;
            existente.Idprueba = pruebaProducto.PruebaId;
            existente.Comentario = pruebaProducto.Comentario;
            existente.ValorEsperado = pruebaProducto.ValorEsperado;
            existente.Tolerancia = pruebaProducto.Tolerancia;

            _context.PruebaProductos.Update(existente);
            await _context.SaveChangesAsync();

            return Ok("Producto Actualizado");
        }
    }
}
