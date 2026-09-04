using System.ComponentModel.DataAnnotations;

namespace HospitalApi.Models;

public class PacienteRequest
{
    [Required]
    [StringLength(50, MinimumLength = 1)]
    public string TipoDocumento { get; set; } = string.Empty;

    [Required]
    [StringLength(50, MinimumLength = 5)]
    [RegularExpression("^[A-Za-z0-9-]+$", ErrorMessage = "El numero de documento solo puede contener letras, numeros y guiones.")]
    public string NumeroDocumento { get; set; } = string.Empty;

    [Required]
    [StringLength(150, MinimumLength = 2)]
    public string Nombre { get; set; } = string.Empty;

    [Required]
    public DateTime FechaNacimiento { get; set; }

    [Required]
    [EmailAddress]
    [StringLength(100)]
    public string Correo { get; set; } = string.Empty;

    [Required]
    [StringLength(10)]
    public string Genero { get; set; } = string.Empty;

    [StringLength(200)]
    public string? Direccion { get; set; }

    [StringLength(20)]
    [RegularExpression("^[0-9+() -]*$", ErrorMessage = "El telefono contiene caracteres no validos.")]
    public string? Telefono { get; set; }

    public bool Activo { get; set; } = true;
}
