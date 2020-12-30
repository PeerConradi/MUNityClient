using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MUNityClient.Models.Simulation
{

    [Obsolete("Use the MUNityBase /MUNitySchema Package!")]
    public class SimulationToken
    {
        public int SimulationId { get; set; }

        public string Name { get; set; }

        public string Token { get; set; }
    }

    [Obsolete("Use the MUNityBase /MUNitySchema Package!")]
    public class SimulationTokenWithPin : SimulationToken
    {
        public string Pin { get; set; }
    }
}
