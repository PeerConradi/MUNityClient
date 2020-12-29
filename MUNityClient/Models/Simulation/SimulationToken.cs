using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MUNityClient.Models.Simulation
{
    public class SimulationToken
    {
        public int SimulationId { get; set; }

        public string Name { get; set; }

        public string Token { get; set; }
    }

    public class SimulationTokenWithPin : SimulationToken
    {
        public string Pin { get; set; }
    }
}
