using Abstracciones.Agentes;
using Abstracciones.Clientes;
using Abstracciones.Comunes;
using Abstracciones.Excepciones;
using Flujo.Clientes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Flujo.Agentes
{
    public class AgenteFlujo : IAgenteFlujo
    {

        private readonly IAgenteDA _agenteDA;
        private readonly TimeProvider _reloj;

        public AgenteFlujo(IAgenteDA agenteDA, TimeProvider reloj)
        {
            _agenteDA = agenteDA;
            _reloj = reloj;
        }

        public async Task ActualizarAsync(int id, AgenteActualizarDTO dto, CancellationToken ct)
        {
            var Correo = NormalizarCorreo(dto.Correo);

            _ = await _agenteDA.ObtenerPorIdAsync(id, ct) ?? throw new NoEncontradoException("Agente", id);

            // El correo puede repetirse solo si pertenece al MISMO cliente que se está editando.
            var otro = await _agenteDA.ObtenerPorCorreoAsync(Correo, ct);
            if (otro is not null && otro.Id != id)
                throw new ConflictoException($"Ya existe otro agente con el correo '{Correo}'.");

            var actualizado = await _agenteDA.ActualizarAsync(new Agente
            {
                Id = id,
                Nombre = dto.Nombre.Trim(),
                Correo = (string)Correo,
                Telefono = dto.Telefono.Trim(),
                CapacidadMaxima = dto.CapacidadMaxima,
                Activo = dto.Activo,
            }, ct);

            if (!actualizado) throw new NoEncontradoException("Agente", id);
        }
        public async Task<AgenteDto> CrearAsync(AgenteCrearDTO dto, CancellationToken ct)
        {
            var correo = NormalizarCorreo(dto.Correo);

            if (await _agenteDA.ObtenerPorCorreoAsync(correo, ct) is not null)
                throw new ConflictoException($"Ya existe un agente con el correo '{correo}'.");

            var agente = new Agente
            {
                Nombre = dto.Nombre.Trim(),
                Correo = correo,
                Telefono = dto.Telefono.Trim(),
                CapacidadMaxima = dto.CapacidadMaxima,
                FechaIngreso = _reloj.GetUtcNow().UtcDateTime
            };

            await _agenteDA.AgregarAsync(agente, ct);
            return AgenteMapeador.ADto(agente);
        }

        public async Task EliminarAsync(int id, CancellationToken ct)
        {
            _ = await _agenteDA.ObtenerPorIdAsync(id, ct) ?? throw new NoEncontradoException("Agente", id);

            if (await _agenteDA.TieneTicketsAsync(id, ct))
                throw new ConflictoException("El agente tiene tickets asociados y no se puede eliminar.");

            if (!await _agenteDA.EliminarAsync(id, ct))
                throw new NoEncontradoException("Agente", id);
        }

        public async Task<ResultadoPaginado<AgenteDto>> ListarAsync(AgenteFiltro filtro, CancellationToken ct)
        {
            var pagina = await _agenteDA.BuscarAsync(filtro, ct);
            return new ResultadoPaginado<AgenteDto>
            {
                Items = pagina.Items.Select(AgenteMapeador.ADto).ToList(),
                Total = pagina.Total,
                Pagina = pagina.Pagina,
                Tamano = pagina.Tamano
            };
        }

        public async Task<AgenteDto> ObtenerAsync(int id, CancellationToken ct)
        {
            var agente = await _agenteDA.ObtenerPorIdAsync(id, ct)
           ?? throw new NoEncontradoException("Agente", id);
            return AgenteMapeador.ADto(agente);
        }


        private static string? LimpiarOpcional(string? texto) =>
       string.IsNullOrWhiteSpace(texto) ? null : texto.Trim();

        private static string NormalizarCorreo(string correo) => correo.Trim().ToLowerInvariant();

    }
}
