using ApiMonitoreo.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace ApiMonitoreo.Controllers
{
    public class PruebaInyectDTO
    {
        public string NoSerie { get; set; }
        public int IdPrueba { get; set; }
        public decimal ValorMedido { get; set; }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class RegistroPruebasController : ControllerBase
    {
        private readonly MonitoreoContext _context;

        public RegistroPruebasController(MonitoreoContext context)
        {
            _context = context;
        }

        [HttpPost("RealizarPrueba")]
        public async Task<IActionResult> Post([FromBody] PruebaInyectDTO valores)
        {
            var serie = await _context.SerieProductos
                .FirstOrDefaultAsync(s => s.NumeroSerie == valores.NoSerie && s.EstatusCalidad == "Pendiente");

            var pruebaExistente = await _context.HistorialPruebas.FirstOrDefaultAsync(p => p.Idprueba == valores.IdPrueba);

            if (serie == null || pruebaExistente != null)
                return NotFound("El numero de serie proporcionado no se le puede realizar la prueba que solicita");

            var reg = new HistorialPrueba();
            reg.Idprueba = valores.IdPrueba;
            reg.SerieId = serie.SerieId;
            reg.ValorMedido = valores.ValorMedido;

            await _context.HistorialPruebas.AddAsync(reg);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Prueba registrada correctamente" });
        }

        [HttpGet("Resultados/{no}")]
        public IActionResult GetProduct(string no)
        {


            return Ok();
        }
    }
}
