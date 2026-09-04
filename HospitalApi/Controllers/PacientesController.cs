using HospitalApi.Models;
using HospitalApi.Services;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace HospitalApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PacientesController : ControllerBase
{
    private readonly IPacienteService _pacienteService;
    private readonly ILogger<PacientesController> _logger;

    public PacientesController(IPacienteService pacienteService, ILogger<PacientesController> logger)
    {
        _pacienteService = pacienteService;
        _logger = logger;
    }

    [HttpPost]
    [ProducesResponseType(typeof(Paciente), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Crear([FromBody] PacienteRequest request)
    {
        try
        {
            var id = await _pacienteService.CrearAsync(request);
            var paciente = await _pacienteService.ObtenerPorIdAsync(id);

            return CreatedAtAction(nameof(ObtenerPorId), new { id }, paciente);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
        catch (PostgresException ex) when (ex.SqlState == PostgresErrorCodes.UniqueViolation)
        {
            return Conflict(new { mensaje = "Ya existe un paciente con el mismo tipo y numero de documento." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear paciente.");
            return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = "Error interno al crear el paciente." });
        }
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Paciente>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ObtenerTodos()
    {
        try
        {
            var pacientes = await _pacienteService.ObtenerTodosAsync();
            return Ok(pacientes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener pacientes.");
            return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = "Error interno al consultar pacientes." });
        }
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(Paciente), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ObtenerPorId(int id)
    {
        try
        {
            var paciente = await _pacienteService.ObtenerPorIdAsync(id);
            return paciente is null ? NotFound(new { mensaje = "Paciente no encontrado." }) : Ok(paciente);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener paciente {PacienteId}.", id);
            return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = "Error interno al consultar el paciente." });
        }
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Actualizar(int id, [FromBody] PacienteRequest request)
    {
        try
        {
            var actualizado = await _pacienteService.ActualizarAsync(id, request);
            return actualizado ? Ok(new { mensaje = "Paciente actualizado correctamente." }) : NotFound(new { mensaje = "Paciente no encontrado." });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
        catch (PostgresException ex) when (ex.SqlState == PostgresErrorCodes.UniqueViolation)
        {
            return Conflict(new { mensaje = "Ya existe un paciente con el mismo tipo y numero de documento." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar paciente {PacienteId}.", id);
            return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = "Error interno al actualizar el paciente." });
        }
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Eliminar(int id)
    {
        try
        {
            var eliminado = await _pacienteService.EliminarAsync(id);
            return eliminado ? NoContent() : NotFound(new { mensaje = "Paciente no encontrado." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar paciente {PacienteId}.", id);
            return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = "Error interno al eliminar el paciente." });
        }
    }
}
