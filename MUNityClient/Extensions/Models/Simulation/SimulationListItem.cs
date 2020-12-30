using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MUNityClient.Models.Simulation
{

    [Obsolete("Use the MUNityBase /MUNitySchema Package!")]
    public class SimulationListItem
    {
        public int SimulationId { get; set; }

        public string Name { get; set; }

        public bool UsingPassword { get; set; }

        public Simulation.GamePhases Phase { get; set; }
    }
}
