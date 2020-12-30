using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MUNityClient.Models.Simulation
{

    [Obsolete("Use the MUNityBase /MUNitySchema Package!")]
    public class SimulationUser
    {
        public int SimulationUserId { get; set; }

        public string DisplayName { get; set; }

        public int RoleId { get; set; }

        /// <summary>
        /// Kommt nicht aus dem Datenmodell sondern ist eine Interne variable um ggf. kommende
        /// Meldungen zu holen
        /// </summary>
        public string CurrentRequest { get; set; }
    }
}
