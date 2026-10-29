using Abstracciones.Agentes;
using Abstracciones.Clientes;
using Abstracciones.Comunes;
using Abstracciones.Excepciones;
using DA.Clientes;
using DA.Comunes;
using DA.Contexto;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DA.Agentes
{
    public class AgenteDA : IAgenteDA
    {

        private readonly SistemaSoporteContexto _contexto;

        public AgenteDA(SistemaSoporteContexto contexto)
        {
            _contexto = contexto;
        }

        public async Task<bool> ActualizarAsync(Agente datos, CancellationToken ct)
        {
            // Entidad rastreada: EF detecta SOLO las propiedades que cambiaron y genera un UPDATE mínimo.
            var existente = await _contexto.Agentes.FirstOrDefaultAsync(c => c.Id == datos.Id, ct);
            if (existente is null) return false;

            existente.Nombre = datos.Nombre;
            existente.Correo = datos.Correo;
            existente.Telefono = datos.Telefono;
            existente.CapacidadMaxima = datos.CapacidadMaxima;
            existente.Activo = datos.Activo;

            try
            {
                await _contexto.SaveChangesAsync(ct);
            }
            catch (DbUpdateException ex) when (ErroresSql.EsDuplicado(ex))
            {
                throw new ConflictoException($"Ya existe otro cliente con el correo '{datos.Correo}'.");
            }
            return true;
        }

        public async Task<Agente> AgregarAsync(Agente agente, CancellationToken ct)
        {
            agente.Activo = true;
            _contexto.Agentes.Add(agente);
            try
            {
                await _contexto.SaveChangesAsync(ct);
            }
            catch (DbUpdateException ex) when (ErroresSql.EsDuplicado(ex))
            {
                throw new ConflictoException($"Ya existe un agente con el correo '{agente.Correo}'.");
            }
            return agente;
        }

        public async Task<ResultadoPaginado<Agente>> BuscarAsync(AgenteFiltro filtro, CancellationToken ct)
        {
            // AsNoTracking: solo leemos, EF no necesita vigilar los objetos.
            IQueryable<Agente> consulta = _contexto.Agentes.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(filtro.Q))
            {
                var texto = filtro.Q.Trim();
                consulta = consulta.Where(c =>
                    c.Nombre.Contains(texto) ||
                    c.Correo.Contains(texto));
            }

            var total = await consulta.CountAsync(ct);

            // Orden estable (Nombre, luego Id) para que la paginación no repita ni se salte filas.
            var items = await consulta
                .OrderBy(c => c.Nombre).ThenBy(c => c.Id)
                .Skip((filtro.Pagina - 1) * filtro.Tamano)
                .Take(filtro.Tamano)
                .ToListAsync(ct);

            return new ResultadoPaginado<Agente>
            {
                Items = items,
                Total = total,
                Pagina = filtro.Pagina,
                Tamano = filtro.Tamano
            };
        }

        public async Task<bool> EliminarAsync(int id, CancellationToken ct)
        {
            var existente = await _contexto.Agentes.FirstOrDefaultAsync(a => a.Id == id, ct);
            if (existente is null) return false;

            _contexto.Agentes.Remove(existente);
            try
            {
                await _contexto.SaveChangesAsync(ct);
            }
            catch (DbUpdateException ex) when (ErroresSql.EsLlaveForanea(ex))
            {
                throw new ConflictoException("El agente tiene tickets asociados y no se puede eliminar.");
            }
            return true;
        }

        public Task<Agente?> ObtenerPorCorreoAsync(string correo, CancellationToken ct) =>
            _contexto.Agentes.AsNoTracking().FirstOrDefaultAsync(c => c.Correo == correo, ct);

        public Task<Agente?> ObtenerPorIdAsync(int id, CancellationToken ct) =>
            _contexto.Agentes.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id, ct);

        public Task<bool> TieneTicketsAsync(int agenteId, CancellationToken ct) =>
            _contexto.Tickets.AnyAsync(t => t.AgenteId == agenteId, ct);
    }
}
