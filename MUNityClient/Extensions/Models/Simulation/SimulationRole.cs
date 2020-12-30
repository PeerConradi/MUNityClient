using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MUNityClient.Models.Simulation
{

    [Obsolete("Use the MUNityBase /MUNitySchema Package!")]
    public class SimulationRole
    {
        public enum RoleTypes
        {
            Spectator,
            Chairman,
            Delegate,
            Moderator
        }

        public int SimulationRoleId { get; set; }
        public string Iso { get; set; }
        public string Name { get; set; }

        public RoleTypes RoleType { get; set; }

        /// <summary>
        /// Not the real name but the display Names of the users that are in this role.
        /// </summary>
        public IEnumerable<string> Users { get; set; }
    }
}
