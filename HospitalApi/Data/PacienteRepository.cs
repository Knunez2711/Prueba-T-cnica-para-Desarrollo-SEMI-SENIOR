using HospitalApi.Models;
using Npgsql;
using NpgsqlTypes;

namespace HospitalApi.Data;

public class PacienteRepository : IPacienteRepository
{
    private readonly string _connectionString;

    public PacienteRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("HospitalConnection")
            ?? throw new InvalidOperationException("No se encontro la cadena de conexion HospitalConnection.");
    }

    public async Task<int> CrearAsync(PacienteRequest paciente)
    {
        const string sql = """
INSERT INTO pacientes
    (tipo_documento, numero_documento, nombre, fecha_nacimiento, correo, genero, direccion, telefono, email, activo)
VALUES
    (@tipo_documento, @numero_documento, @nombre, @fecha_nacimiento, @correo, @genero, @direccion, @telefono, @email, @activo)
RETURNING id;
""";

        await using var connection = new NpgsqlConnection(_connectionString);
        await using var command = new NpgsqlCommand(sql, connection);
        AgregarParametros(command, paciente);

        await connection.OpenAsync();
        var result = await command.ExecuteScalarAsync();
        return Convert.ToInt32(result);
    }

    public async Task<IReadOnlyList<Paciente>> ObtenerTodosAsync()
    {
        const string sql = """
SELECT id, tipo_documento, numero_documento, nombre, fecha_nacimiento, correo, genero, direccion, telefono, activo, fecha_creacion
FROM pacientes
ORDER BY id DESC;
""";

        var pacientes = new List<Paciente>();

        await using var connection = new NpgsqlConnection(_connectionString);
        await using var command = new NpgsqlCommand(sql, connection);

        await connection.OpenAsync();
        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            pacientes.Add(MapearPaciente(reader));
        }

        return pacientes;
    }

    public async Task<Paciente?> ObtenerPorIdAsync(int id)
    {
        const string sql = """
SELECT id, tipo_documento, numero_documento, nombre, fecha_nacimiento, correo, genero, direccion, telefono, activo, fecha_creacion
FROM pacientes
WHERE id = @id;
""";

        await using var connection = new NpgsqlConnection(_connectionString);
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.Add("@id", NpgsqlDbType.Integer).Value = id;

        await connection.OpenAsync();
        await using var reader = await command.ExecuteReaderAsync();

        return await reader.ReadAsync() ? MapearPaciente(reader) : null;
    }

    public async Task<bool> ActualizarAsync(int id, PacienteRequest paciente)
    {
        const string sql = """
UPDATE pacientes
SET tipo_documento = @tipo_documento,
    numero_documento = @numero_documento,
    nombre = @nombre,
    fecha_nacimiento = @fecha_nacimiento,
    correo = @correo,
    genero = @genero,
    direccion = @direccion,
    telefono = @telefono,
    email = @email,
    activo = @activo
WHERE id = @id;
""";

        await using var connection = new NpgsqlConnection(_connectionString);
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.Add("@id", NpgsqlDbType.Integer).Value = id;
        AgregarParametros(command, paciente);

        await connection.OpenAsync();
        var rows = await command.ExecuteNonQueryAsync();
        return rows > 0;
    }

    public async Task<bool> EliminarAsync(int id)
    {
        const string sql = "DELETE FROM pacientes WHERE id = @id;";

        await using var connection = new NpgsqlConnection(_connectionString);
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.Add("@id", NpgsqlDbType.Integer).Value = id;

        await connection.OpenAsync();
        var rows = await command.ExecuteNonQueryAsync();
        return rows > 0;
    }

    private static void AgregarParametros(NpgsqlCommand command, PacienteRequest paciente)
    {
        var correo = paciente.Correo.Trim();

        command.Parameters.Add("@tipo_documento", NpgsqlDbType.Varchar, 50).Value = paciente.TipoDocumento.Trim();
        command.Parameters.Add("@numero_documento", NpgsqlDbType.Varchar, 50).Value = paciente.NumeroDocumento.Trim();
        command.Parameters.Add("@nombre", NpgsqlDbType.Varchar, 150).Value = paciente.Nombre.Trim();
        command.Parameters.Add("@fecha_nacimiento", NpgsqlDbType.Date).Value = DateOnly.FromDateTime(paciente.FechaNacimiento);
        command.Parameters.Add("@correo", NpgsqlDbType.Varchar, 100).Value = correo;
        command.Parameters.Add("@genero", NpgsqlDbType.Varchar, 10).Value = paciente.Genero.Trim();
        command.Parameters.Add("@direccion", NpgsqlDbType.Varchar, 200).Value = (object?)paciente.Direccion?.Trim() ?? DBNull.Value;
        command.Parameters.Add("@telefono", NpgsqlDbType.Varchar, 20).Value = (object?)paciente.Telefono?.Trim() ?? DBNull.Value;
        command.Parameters.Add("@email", NpgsqlDbType.Varchar, 100).Value = correo;
        command.Parameters.Add("@activo", NpgsqlDbType.Boolean).Value = paciente.Activo;
    }

    private static Paciente MapearPaciente(NpgsqlDataReader reader)
    {
        return new Paciente
        {
            Id = reader.GetInt32(reader.GetOrdinal("id")),
            TipoDocumento = reader.GetString(reader.GetOrdinal("tipo_documento")),
            NumeroDocumento = reader.GetString(reader.GetOrdinal("numero_documento")),
            Nombre = reader.GetString(reader.GetOrdinal("nombre")),
            FechaNacimiento = reader.GetDateTime(reader.GetOrdinal("fecha_nacimiento")),
            Correo = reader.GetString(reader.GetOrdinal("correo")),
            Genero = reader.GetString(reader.GetOrdinal("genero")),
            Direccion = reader.IsDBNull(reader.GetOrdinal("direccion")) ? null : reader.GetString(reader.GetOrdinal("direccion")),
            Telefono = reader.IsDBNull(reader.GetOrdinal("telefono")) ? null : reader.GetString(reader.GetOrdinal("telefono")),
            Activo = reader.GetBoolean(reader.GetOrdinal("activo")),
            FechaCreacion = reader.GetDateTime(reader.GetOrdinal("fecha_creacion"))
        };
    }
}
