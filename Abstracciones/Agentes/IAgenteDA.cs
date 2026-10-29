using Abstracciones.Clientes;
using Abstracciones.Comunes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Abstracciones.Agentes
{
    public interface IAgenteDA
    {

        Task<ResultadoPaginado<Agente>> BuscarAsync(AgenteFiltro filtro, CancellationToken ct);
        Task<Agente?> ObtenerPorIdAsync(int id, CancellationToken ct);
        Task<Agente?> ObtenerPorCorreoAsync(string correo, CancellationToken ct);
        Task<Agente> AgregarAsync(Agente agente, CancellationToken ct);

        /// <summary>Actualiza Nombre, Correo, Telefono, Capacidad Maxima, Activo. Devuelve false si el cliente no existe.</summary>
        Task<bool> ActualizarAsync(Agente datos, CancellationToken ct);

        Task<bool> EliminarAsync(int id, CancellationToken ct);
        Task<bool> TieneTicketsAsync(int agenteId, CancellationToken ct);

        /// <summary>Ejemplo didáctico de procedimiento almacenado con parámetros (sin concatenar SQL).</summary>

    }
}
