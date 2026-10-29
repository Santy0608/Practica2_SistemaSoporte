using Abstracciones.Tickets;
using System;
using System.Collections.Generic;
using System.Text;

namespace Abstracciones.Agentes
{
    public class Agente
    {

        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Correo { get; set; }
        public string Telefono { get; set; }
        public int CapacidadMaxima { get; set; }
        public bool Activo { get; set; }
        public DateTime FechaIngreso { get; set; }

        public List<Ticket> Tickets { get; set; } = new();

    }
}
