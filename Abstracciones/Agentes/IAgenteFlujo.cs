using Abstracciones.Clientes;
using Abstracciones.Comunes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Abstracciones.Agentes
{
    public interface IAgenteFlujo
    {

        Task<ResultadoPaginado<AgenteDto>> ListarAsync(AgenteFiltro filtro, CancellationToken ct);
        Task<AgenteDto> ObtenerAsync(int id, CancellationToken ct);
        Task<AgenteDto> CrearAsync(AgenteCrearDTO dto, CancellationToken ct);
        Task ActualizarAsync(int id, AgenteActualizarDTO dto, CancellationToken ct);
        Task EliminarAsync(int id, CancellationToken ct);

    }
}
