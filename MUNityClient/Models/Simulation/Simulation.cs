using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MUNityClient.Models.Simulation
{

    [Obsolete("Use the MUNityBase /MUNitySchema Package!")]
    public class Simulation
    {
        public enum LobbyModes
        {
            /// <summary>
            /// Allows to Join the game with a role token and will then give
            /// you the role.
            /// </summary>
            WithRoleKey,
            /// <summary>
            /// You can join the lobby and pick a role
            /// </summary>
            PickRole,
            /// <summary>
            /// Allow everyone inside the Lobby to create a role
            /// </summary>
            CreateAnyRole,
            /// <summary>
            /// the lobby is closed you cannot join.
            /// </summary>
            Closed
        }

        public enum GamePhases
        {
            Offline,
            Lobby,
            Online
        }

        public int SimulationId { get; set; }

        public string Name { get; set; }

        public GamePhases Phase { get; set; }

        public LobbyModes LobbyMode { get; set; }

        /// <summary>
        /// Momentaner Status wie Sitzung, Abstimmung oder informelle Sitzung Pause etc. als Text.
        /// </summary>
        public string Status { get; set; }

        public List<SimulationRole> Roles { get; set; }

        public List<SimulationUser> Users { get; set; }
    }
}
