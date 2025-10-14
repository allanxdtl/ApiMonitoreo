using ApiMonitoreo.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace ApiMonitoreo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PruebasController : ControllerBase
    {
        private readonly MonitoreoContext _context;
        public PruebasController(MonitoreoContext context)
        {
            _context = context;
        }

        [HttpGet("List")]
        public async Task<IActionResult> Get()
        {
            return Ok(await _context.Pruebas.ToListAsync());
        }

        [HttpPost("Insert")]
        public async Task<IActionResult> Insert([FromBody] Prueba prueba)
        {
            await _context.Pruebas.AddAsync(prueba);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Prueba insertada correctamente" });
        }

        [HttpPut("Edit")]
        public async Task<IActionResult> Edit([FromBody] Prueba prueba)
        {
            _context.Pruebas.Update(prueba);

            await _context.SaveChangesAsync();

            return Ok(new { message = "Prueba editada exitosamente" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _context.Pruebas.FindAsync(id);

            if (result == null)
                return NotFound();

            _context.Pruebas.Remove(result);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Prueba eliminada exitosamente" });
        }
    }
}
