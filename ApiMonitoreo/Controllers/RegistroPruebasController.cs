using ApiMonitoreo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
                .FirstOrDefaultAsync(s => s.NumeroSerie == valores.NoSerie);

            var pruebaExistente = await _context.HistorialPruebas
                .FirstOrDefaultAsync(p =>
                    p.Idprueba == valores.IdPrueba &&
                    p.SerieId == serie.SerieId
                );


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

        [HttpGet("Resultados/{noSerie}")]
        public async Task<IActionResult> GetProduct(string noSerie)
        {
            var result = await _context.HistorialPruebas
                                    .Join(_context.SerieProductos, hp => hp.SerieId, sp => sp.SerieId, (hp, sp) => new { hp, sp })
                                    .Where(s => s.sp.NumeroSerie == noSerie)
                                    .Join(_context.Pruebas, x => x.hp.Idprueba, p => p.Idprueba, (x, p) => new { x.hp, x.sp, p })
                                    .Join(_context.PruebaProductos, x => x.p.Idprueba, pp => pp.Idprueba, (x, pp) => new
                                    {
                                        Descripcion = x.p.Descripcion,
                                        ValorMedido = x.hp.ValorMedido,
                                        ValorEsperado = pp.ValorEsperado,
                                        Tolerancia = pp.Tolerancia
                                    }).ToListAsync();

            return Ok(result);
        }
    }
}
