using Abstracciones.Comunes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Abstracciones.Agentes
{
    public class AgenteCrearDTO
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 100 caracteres.")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo es obligatorio.")]
        [EmailAddress(ErrorMessage = "El correo no tiene un formato válido.")]
        [StringLength(150)]
        public string Correo { get; set; } = string.Empty;

        [Required(ErrorMessage = "El teléfono es obligatorio.")]
        [RegularExpression(@"^\d{8}$", ErrorMessage = "El teléfono debe tener exactamente 8 dígitos.")]
        public string Telefono { get; set; } = string.Empty;
        public int CapacidadMaxima { get; set; }
        public bool Activo { get; set; }

    }

    public class AgenteActualizarDTO : AgenteCrearDTO { }

    /// <summary>Salida hacia el cliente HTTP. Nunca se expone la entidad directamente.</summary>
    public record AgenteDto(
        int Id,
        string Nombre,
        string Correo,
        string Telefono,
        int CapacidadMaxima,
        bool Activo);

    public class AgenteFiltro : ParametrosPaginacion
    {
        /// <summary>Texto buscado en nombre, correo o empresa.</summary>
        [StringLength(100)]
        public string? Q { get; set; }
    }
}
