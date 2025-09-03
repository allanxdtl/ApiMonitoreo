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

		//Recibe en el body un objeto de tipo usuario, se puede mandar solo el campo de Usuario y Password
		[HttpPost("Login")]
		public async Task<IActionResult> Login([FromBody] Usuario user)
		{
			if (user.Id < 0)
				return BadRequest("La estructura de la peticion esta mal");

			//Si el usuario y contraseña son correctos devuelve el mensaje de Bienvenido
			Usuario? usuario = await _monitoreo.Usuarios.FirstOrDefaultAsync(u => u.Usuario1 == user.Usuario1 && u.Password == user.Password);

			//Si no existe, regresa un status 404
			if (usuario == null)
				return NotFound("Usuario o contraseña incorrectos");

			return Ok("Bienvenido");
		}

		//Recibe en el body un objeto de tipo usuario, se pueden mandar todos los campos, con el id = 0
		[HttpPost("Crear")]
		public async Task<IActionResult> CrearUsuario([FromBody] Usuario user)
		{
			await _monitoreo.Usuarios.AddAsync(user);
			await _monitoreo.SaveChangesAsync();

			return Ok("Usuario creado");
		}

	}
}
