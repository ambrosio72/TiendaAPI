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
    public class ProductosController : ControllerBase
    {
        private readonly TiendaDbContext _context;
        private readonly ILogger<ProductosController> _logger;

        public ProductosController(TiendaDbContext context, ILogger<ProductosController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/Productos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Producto>>> GetProductos()
        {
            try
            {
                return await _context.Productos.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la lista de productos.");
                return StatusCode(500, "Error interno del servidor.");
            }
        }

        // GET: api/Productos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Producto>> GetProducto(int id)
        {
            try
            {
                var producto = await _context.Productos.FindAsync(id);

                if (producto == null)
                {
                    return NotFound(); // 404
                }

                return producto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el producto con ID {id}.", id);
                return StatusCode(500, "Error interno del servidor.");
            }
        }

        // POST: api/Productos
        [HttpPost]
        public async Task<ActionResult<Producto>> PostProducto(Producto producto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState); // 400
                }

                _context.Productos.Add(producto);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetProducto), new { id = producto.Id }, producto); // 201
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear un nuevo producto.");
                return StatusCode(500, "Error interno del servidor.");
            }
        }

        // PUT: api/Productos/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProducto(int id, Producto producto)
        {
            if (id != producto.Id)
            {
                return BadRequest(); // 400
            }

            _context.Entry(producto).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductoExists(id))
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
                _logger.LogError(ex, "Error al actualizar el producto con ID {id}.", id);
                return StatusCode(500, "Error interno del servidor.");
            }

            return NoContent(); // 204
        }

        // DELETE: api/Productos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProducto(int id)
        {
            try
            {
                var producto = await _context.Productos.FindAsync(id);
                if (producto == null)
                {
                    return NotFound(); // 404
                }

                _context.Productos.Remove(producto);
                await _context.SaveChangesAsync();

                return NoContent(); // 204
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el producto con ID {id}.", id);
                return StatusCode(500, "Error interno del servidor.");
            }
        }

        // Consultas específicas:

        // GET: api/Productos/precio/{precio}
        [HttpGet("precio/{precio}")]
        public async Task<ActionResult<IEnumerable<Producto>>> GetProductosPorPrecio(decimal precio)
        {
            try
            {
                var productos = await _context.Productos.Where(p => p.Precio == precio).ToListAsync();
                if (productos == null || !productos.Any())
                {
                    return NotFound(); // 404
                }
                return productos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener productos con precio {precio}.", precio);
                return StatusCode(500, "Error interno del servidor.");
            }
        }

        // GET: api/Productos/stockbajo
        [HttpGet("stockbajo")]
        public async Task<ActionResult<IEnumerable<Producto>>> GetProductosConStockBajo()
        {
            try
            {
                var productos = await _context.Productos.Where(p => p.Stock < 10).ToListAsync();
                if (productos == null || !productos.Any())
                {
                    return NotFound(); // 404
                }
                return productos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener productos con stock bajo.");
                return StatusCode(500, "Error interno del servidor.");
            }
        }

        // GET: api/Productos/buscar/{palabra}
        [HttpGet("buscar/{palabra}")]
        public async Task<ActionResult<IEnumerable<Producto>>> BuscarProductosPorDescripcion(string palabra)
        {
            try
            {
                var productos = await _context.Productos.Where(p => p.Descripcion.Contains(palabra)).ToListAsync();
                if (productos == null || !productos.Any())
                {
                    return NotFound(); // 404
                }
                return productos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar productos con la palabra '{palabra}' en la descripción.", palabra);
                return StatusCode(500, "Error interno del servidor.");
            }
        }

        // Consultas con ordenación:

        // GET: api/Productos/precio/{orden}
        [HttpGet("precio/{orden}")]
        public async Task<ActionResult<IEnumerable<Producto>>> GetProductosPorPrecioOrdenado(string orden)
        {
            try
            {
                IQueryable<Producto> productos = _context.Productos;

                if (orden.ToLower() == "asc")
                {
                    productos = productos.OrderBy(p => p.Precio);
                }
                else if (orden.ToLower() == "desc")
                {
                    productos = productos.OrderByDescending(p => p.Precio);
                }
                else
                {
                    return BadRequest("Orden inválido. Use 'asc' o 'desc'."); // 400
                }

                return await productos.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener productos ordenados por precio ({orden}).", orden);
                return StatusCode(500, "Error interno del servidor.");
            }
        }

        private bool ProductoExists(int id)
        {
            return _context.Productos.Any(e => e.Id == id);
        }
    }
}
