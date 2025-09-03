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

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] Usuario user)
        {
            if (user.Id < 0)
                return BadRequest("La estructura de la peticion esta mal");

            Usuario? usuario = await _monitoreo.Usuarios.FirstOrDefaultAsync(u => u.Usuario1 == user.Usuario1 && u.Password==user.Password);

            if (usuario == null)
                return NotFound("Usuario o contraseña incorrectos");

            return Ok("Bienvenido");

        }

    }
}
