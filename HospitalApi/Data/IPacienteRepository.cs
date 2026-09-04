using HospitalApi.Models;

namespace HospitalApi.Data;

public interface IPacienteRepository
{
    Task<int> CrearAsync(PacienteRequest paciente);
    Task<IReadOnlyList<Paciente>> ObtenerTodosAsync();
    Task<Paciente?> ObtenerPorIdAsync(int id);
    Task<bool> ActualizarAsync(int id, PacienteRequest paciente);
    Task<bool> EliminarAsync(int id);
}
