using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MUNityClient.Models.Simulation;

namespace MUNityClient.Services.SocketHandlers
{
    public class SimulationSocketHandler
    {
        public delegate void OnRolesChanged(int sender, IEnumerable<SimulationRole> roles);
        public event OnRolesChanged RolesChanged;

        public delegate void OnUserRoleChanged(int sender, int userId, int roleId);
        public event OnUserRoleChanged UserRoleChanged;

        public delegate void OnUserConnected(int sender, SimulationUser user);
        public event OnUserConnected UserConnected;

        public delegate void OnUserDisconnected(int sender, SimulationUser user);
        public event OnUserDisconnected UserDisconnected;

        public HubConnection HubConnection { get; set; }

        private readonly int _simulationId;

        public SimulationSocketHandler()
        {
            HubConnection = new HubConnectionBuilder().WithUrl($"{Program.API_URL}/simsocket").Build();
            HubConnection.On<int, IEnumerable<SimulationRole>>("RolesChanged", (id, roles) => RolesChanged?.Invoke(id, roles));
            HubConnection.On<int, int, int>("UserRoleChanged", (simId, userId, roleId) => UserRoleChanged?.Invoke(simId, userId, roleId));
            HubConnection.On<int, SimulationUser>("UserConnected", (id, user) => UserConnected?.Invoke(id, user));
            HubConnection.On<int, SimulationUser>("UserDisconnected", (id, user) => UserDisconnected?.Invoke(id, user));
        }


    }
}
