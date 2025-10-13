using ApiMonitoreo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiMonitoreo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductoTerminadoController : ControllerBase
    {
        private readonly MonitoreoContext _context;
        //Inyeccion de dependencia
        public ProductoTerminadoController(MonitoreoContext context)
        {
            _context = context;
        }

        [HttpGet("GetByNoSerie/{str}")]
        public async Task<IActionResult> GetByNoSerie(string str)
        {
            var resultado = await
                            (from p in _context.Produccions
                             join sp in _context.SerieProductos on p.ProduccionId equals sp.ProduccionId
                             join pt in _context.ProductoTerminados on p.ProductoId equals pt.ProductoId
                             where sp.NumeroSerie == str
                             select new
                             {
                                 ID = pt.ProductoId,
                                 Nombre = pt.Nombre,
                                 FechaDeProduccion = p.FechaProduccion
                             }).ToListAsync();

            return Ok(resultado);
        }

        [HttpGet("List")]
        public async Task<IActionResult> Get()
        {
            var productos = await _context.ProductoTerminados
                .Select(p => new
                {
                    p.ProductoId,
                    p.Nombre
                })
                .ToListAsync();
            return Ok(productos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            // Intentar parsear el ID como entero
            bool esNumero = int.TryParse(id, out int productoId);

            var producto = await _context.ProductoTerminados
                .FirstOrDefaultAsync(p =>
                    p.Nombre == id || (esNumero && p.ProductoId == productoId));

            if (producto == null)
                return NotFound();

            return Ok(new { producto.ProductoId, producto.Nombre });
        }

        [HttpPost("Insert")]
        public async Task<IActionResult> Insert(ProductoTerminado producto)
        {
            _context.ProductoTerminados.Add(producto);
            await _context.SaveChangesAsync();
            return Ok(new { producto.ProductoId, producto.Nombre });
        }

        [HttpPut("Edit")]
        public async Task<IActionResult> Update(ProductoTerminado producto)
        {
            if (producto == null || producto.ProductoId <= 0)
                return BadRequest();

            ProductoTerminado? productoActual = await _context.ProductoTerminados.FindAsync(producto.ProductoId);

            if (productoActual == null)
                return NotFound();

            productoActual.Nombre = producto.Nombre;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var producto = await _context.ProductoTerminados.FindAsync(id);
            if (producto == null)
                return NotFound();
            _context.ProductoTerminados.Remove(producto);
            await _context.SaveChangesAsync();
            return NoContent();
        }

    }
}
