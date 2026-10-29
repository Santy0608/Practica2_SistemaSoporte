using Abstracciones.Agentes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Flujo.Agentes
{
    public class AgenteMapeador
    {

        public static AgenteDto ADto(Agente a) =>
            new(a.Id, a.Nombre, a.Correo, a.Telefono, a.CapacidadMaxima, a.Activo ,a.FechaIngreso);

    }
}
