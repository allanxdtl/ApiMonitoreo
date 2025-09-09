using ApiMonitoreo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System.Threading.Tasks;

namespace ApiMonitoreo.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class ClienteController : ControllerBase
	{
		private readonly MonitoreoContext _monitoreo;
		//Inyeccion de dependencia
		public ClienteController(MonitoreoContext monitoreo)
		{
			_monitoreo = monitoreo;
		}

		//Obtiene la lista de clientes
		[HttpGet("List")]
		public async Task<IActionResult> Get()
		{
			var list = await _monitoreo.Clientes.ToListAsync();
			return Ok(list);
		}

		[HttpGet("Buscar/{text}")]
		public async Task<IActionResult> Buscar(string text)
		{
			var list = await _monitoreo.Clientes
				.Where(c => c.RazonSocial.Contains(text) || c.Cp.Contains(text))
				.ToListAsync();
			return Ok(list);
		}

		//Obtiene un cliente por su id
		[HttpGet("{id}")]
		public async Task<IActionResult> GetById(int id)
		{
			var cliente = await _monitoreo.Clientes.FindAsync(id);
			if (cliente == null)
				return NotFound();
			return Ok(cliente);
		}

		//Inserta un nuevo cliente, se pueden mandar todos los campos, con el id = 0
		[HttpPost("Insert")]
		public async Task<IActionResult> Insert([FromBody] Cliente newCliente)
		{
			await _monitoreo.Clientes.AddAsync(newCliente);
			await _monitoreo.SaveChangesAsync();
			return Ok("Cliente insertado");
		}

		[HttpPut("Update")]
		public async Task<IActionResult> Update([FromBody] Cliente updatedCliente)
		{
			//Busca el cliente recibido por su id
			var existingCliente = await _monitoreo.Clientes.FindAsync(updatedCliente.Id);

			//Si no lo encuentra, regresa un status 404
			if (existingCliente == null)
				return NotFound("Cliente no encontrado");

			//Si existe, actualiza los campos
			existingCliente.RazonSocial = updatedCliente.RazonSocial;
			existingCliente.Cp = updatedCliente.Cp;

			_monitoreo.Clientes.Update(existingCliente);

			await _monitoreo.SaveChangesAsync(); //Guarda los cambios en la base de datos

			return Ok("Cliente actualizado"); //Regresa status 200 con mensaje de éxito
		}

		[HttpDelete("Delete/{id}")]
		public async Task<IActionResult> Delete(int id)
		{
			//Busca el cliente por su id
			var cliente = await _monitoreo.Clientes.FindAsync(id);

			//Si no lo encuentra, regresa un status 404
			if (cliente == null)
				return NotFound("Cliente no encontrado");

			//Si lo encuentra, lo elimina
			_monitoreo.Clientes.Remove(cliente);
			await _monitoreo.SaveChangesAsync(); //Guarda los cambios en la base de datos

			return Ok("Cliente eliminado"); //Regresa status 200 con mensaje de éxito
		}
	}
}
