using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TiendaAPI.Models; // Reemplaza con el nombre de tu proyecto
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;
using TiendaAPI.Data;

namespace TiendaAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly TiendaDbContext _context;
        private readonly ILogger<UsuariosController> _logger;

        public UsuariosController(TiendaDbContext context, ILogger<UsuariosController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/Usuarios
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Usuario>>> GetUsuarios()
        {
            try
            {
                return await _context.Usuarios.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la lista de usuarios.");
                return StatusCode(500, "Error interno del servidor.");
            }
        }

        // GET: api/Usuarios/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Usuario>> GetUsuario(int id)
        {
            try
            {
                var usuario = await _context.Usuarios.FindAsync(id);

                if (usuario == null)
                {
                    return NotFound(); // 404
                }

                return usuario;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el usuario con ID {id}.", id);
                return StatusCode(500, "Error interno del servidor.");
            }
        }

        // POST: api/Usuarios
        [HttpPost]
        public async Task<ActionResult<Usuario>> PostUsuario(Usuario usuario)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState); // 400
                }

                _context.Usuarios.Add(usuario);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetUsuario), new { id = usuario.Id }, usuario); // 201
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear un nuevo usuario.");
                return StatusCode(500, "Error interno del servidor.");
            }
        }

        // PUT: api/Usuarios/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUsuario(int id, Usuario usuario)
        {
            if (id != usuario.Id)
            {
                return BadRequest(); // 400
            }

            _context.Entry(usuario).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UsuarioExists(id))
                {
                    return NotFound(); // 404
                }
                else
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el usuario con ID {id}.", id);
                return StatusCode(500, "Error interno del servidor.");
            }

            return NoContent(); // 204
        }

        // DELETE: api/Usuarios/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsuario(int id)
        {
            try
            {
                var usuario = await _context.Usuarios.FindAsync(id);
                if (usuario == null)
                {
                    return NotFound(); // 404
                }

                _context.Usuarios.Remove(usuario);
                await _context.SaveChangesAsync();

                return NoContent(); // 204
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el usuario con ID {id}.", id);
                return StatusCode(500, "Error interno del servidor.");
            }
        }

        // Consultas específicas:

        // GET: api/Usuarios/nombre/{nombre}
        [HttpGet("nombre/{nombre}")]
        public async Task<ActionResult<IEnumerable<Usuario>>> GetUsuariosPorNombre(string nombre)
        {
            try
            {
                var usuarios = await _context.Usuarios.Where(u => u.NombreUsuario.Contains(nombre)).ToListAsync();
                if (usuarios == null || !usuarios.Any())
                {
                    return NotFound(); // 404
                }
                return usuarios;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener usuarios con nombre '{nombre}'.", nombre);
                return StatusCode(500, "Error interno del servidor.");
            }
        }

        // GET: api/Usuarios/dominio/{dominio}
        [HttpGet("dominio/{dominio}")]
        public async Task<ActionResult<IEnumerable<Usuario>>> GetUsuariosPorDominioCorreo(string dominio)
        {
            try
            {
                var usuarios = await _context.Usuarios.Where(u => u.CorreoElectronico.EndsWith(dominio)).ToListAsync();
                if (usuarios == null || !usuarios.Any())
                {
                    return NotFound(); // 404
                }
                return usuarios;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener usuarios con dominio de correo '{dominio}'.", dominio);
                return StatusCode(500, "Error interno del servidor.");
            }
        }

        // Consultas con ordenación:

        // GET: api/Usuarios/nombre/{orden}
        [HttpGet("nombre/{orden}")]
        public async Task<ActionResult<IEnumerable<Usuario>>> GetUsuariosPorNombreOrdenado(string orden)
        {
            try
            {
                IQueryable<Usuario> usuarios = _context.Usuarios;

                if (orden.ToLower() == "asc")
                {
                    usuarios = usuarios.OrderBy(u => u.NombreUsuario);
                }
                else if (orden.ToLower() == "desc")
                {
                    usuarios = usuarios.OrderByDescending(u => u.NombreUsuario);
                }
                else
                {
                    return BadRequest("Orden inválido. Use 'asc' o 'desc'."); // 400
                }

                return await usuarios.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener usuarios ordenados por nombre ({orden}).", orden);
                return StatusCode(500, "Error interno del servidor.");
            }
        }

        private bool UsuarioExists(int id)
        {
            return _context.Usuarios.Any(e => e.Id == id);
        }

        // Endpoint adicional: GET: api/Usuarios/correo/{correoElectronico}
        [HttpGet("correo/{correoElectronico}")]
        public async Task<ActionResult<Usuario>> GetUsuarioPorCorreo(string correoElectronico)
        {
            try
            {
                var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.CorreoElectronico == correoElectronico);
                if (usuario == null)
                {
                    return NotFound(); // 404
                }
                return usuario;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener usuario con correo '{correoElectronico}'.", correoElectronico);
                return StatusCode(500, "Error interno del servidor.");
            }
        }
    }
}
