using ApiMonitoreo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace ApiMonitoreo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioController : ControllerBase
    {
        private readonly MonitoreoContext _monitoreo;
        //Inyeccion de dependencia
        public UsuarioController(MonitoreoContext context)
        {
            _monitoreo = context;
        }

        [HttpGet("Login")]
        public async Task<IActionResult> Login()
        {
            return Ok(await _monitoreo.Usuarios.ToListAsync());
        }

    }
}
