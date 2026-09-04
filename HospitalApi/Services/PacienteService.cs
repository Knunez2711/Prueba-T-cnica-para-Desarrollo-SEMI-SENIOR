using HospitalApi.Data;
using HospitalApi.Models;

namespace HospitalApi.Services;

public class PacienteService : IPacienteService
{
    private readonly IPacienteRepository _pacienteRepository;

    public PacienteService(IPacienteRepository pacienteRepository)
    {
        _pacienteRepository = pacienteRepository;
    }

    public Task<int> CrearAsync(PacienteRequest paciente)
    {
        ValidarFechaNacimiento(paciente.FechaNacimiento);
        return _pacienteRepository.CrearAsync(paciente);
    }

    public Task<IReadOnlyList<Paciente>> ObtenerTodosAsync()
    {
        return _pacienteRepository.ObtenerTodosAsync();
    }

    public Task<Paciente?> ObtenerPorIdAsync(int id)
    {
        return _pacienteRepository.ObtenerPorIdAsync(id);
    }

    public Task<bool> ActualizarAsync(int id, PacienteRequest paciente)
    {
        ValidarFechaNacimiento(paciente.FechaNacimiento);
        return _pacienteRepository.ActualizarAsync(id, paciente);
    }

    public Task<bool> EliminarAsync(int id)
    {
        return _pacienteRepository.EliminarAsync(id);
    }

    private static void ValidarFechaNacimiento(DateTime fechaNacimiento)
    {
        if (fechaNacimiento.Date > DateTime.UtcNow.Date)
        {
            throw new ArgumentException("La fecha de nacimiento no puede ser futura.");
        }
    }
}
