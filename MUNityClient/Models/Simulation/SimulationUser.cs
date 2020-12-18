using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MUNityClient.Models.Simulation
{
    public class SimulationUser
    {
        public int SimulationUserId { get; set; }

        public string DisplayName { get; set; }

        public int RoleId { get; set; }
    }
}
