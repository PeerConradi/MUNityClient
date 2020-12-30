using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MUNityClient.Models.Simulation
{

    [Obsolete("Use the MUNityBase /MUNitySchema Package!")]
    public class SimulationAuth
    {
        public int SimulationUserId { get; set; }

        public bool CanCreateRole { get; set; }

        public bool CanSelectRole { get; set; }

        public bool CanEditResolution { get; set; }

        public bool CanEditListOfSpeakers { get; set; }
    }
}
