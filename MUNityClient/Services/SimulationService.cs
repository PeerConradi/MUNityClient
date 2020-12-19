using MUNityClient.Models.Simulation;
using MUNityClient.Models.Simulation.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http.Json;
using Blazored.LocalStorage;
using System.Net.Http;

namespace MUNityClient.Services
{
    public class SimulationService
    {
        private readonly HttpService _httpService;

        private readonly ILocalStorageService _localStorage;

        public async Task<bool> IsOnline()
        {
            try
            {
                var result = await this._httpService.HttpClient.GetAsync($"/api/Simulation/IsOnline");
                if (result.IsSuccessStatusCode)
                    return true;
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Creates a Simulation and returns its token. The token will also be stored inside the 
        /// munity_simsims local storage.
        /// </summary>
        /// <param name="schema"></param>
        /// <returns></returns>
        public async Task<SimulationTokenWithPin> CreateSimulation(CreateSchema schema)
        {
            var content = JsonContent.Create(schema);
            var result = await this._httpService.HttpClient.PostAsync($"/api/Simulation/CreateSimulation", content);
            if (result.IsSuccessStatusCode)
            {
                var token = await result.Content.ReadFromJsonAsync<SimulationTokenWithPin>();
                await StoreToken(token);
                return token;
            }
            throw new Exception("something went wrong");
        }

        public async Task<ICollection<SimulationToken>> GetStoredTokens()
        {
            return await this._localStorage.GetItemAsync<ICollection<SimulationToken>>("munity_simsims");
        }

        public async Task<ICollection<SimulationListItem>> GetSimulationList()
        {
            return await this._httpService.HttpClient.GetFromJsonAsync<ICollection<SimulationListItem>>("/api/Simulation/GetListOfSimulations");
        }

        public async Task<SimulationToken> GetSimulationToken(int id)
        {
            var tokens = await GetStoredTokens();
            return tokens.FirstOrDefault(n => n.SimulationId == id);
        }

        public async Task<SimulationTokenWithPin> JoinSimulation(int simulationId, JoinAuthenticate body)
        {
            var content = JsonContent.Create(body);
            var result = await this._httpService.HttpClient.PostAsync($"/api/Simulation/JoinSimulation?id={simulationId}", content);
            if (result.IsSuccessStatusCode)
            {
                var token = await result.Content.ReadFromJsonAsync<SimulationTokenWithPin>();
                await StoreToken(token);
                return token;
            }
            throw new Exception("something went wrong");
        }

        public async Task PickRole(int simulationId, int roleId)
        {
            var client = await GetSimulationClient(simulationId);
            if (client == null) throw new Exception();
            await client.GetAsync($"/api/Simulation/PickRole?simulationId={simulationId}&roleId={roleId}");
        }

        public async Task<HttpResponseMessage> SetPhase(int simulationId, int phase)
        {
            var client = await GetSimulationClient(simulationId);
            if (client == null) throw new Exception();
            return await client.GetAsync($"/api/Simulation/SetPhase?simulationId={simulationId}&phase={phase}");
        }

        public async Task<HttpResponseMessage> MakeRequest(int simulationId, string request)
        {
            var client = await GetSimulationClient(simulationId);
            if (client == null) throw new Exception();
            return await client.GetAsync($"/api/Simulation/MakeRequest?simulationId={simulationId}&request={request}");
        }

        public async Task<HttpResponseMessage> AcceptRequest(int simulationId, int userId, string request)
        {
            var client = await GetSimulationClient(simulationId);
            if (client == null) throw new Exception();
            return await client.GetAsync($"/api/Simulation/AcceptRequest?simulationId={simulationId}&userId={userId}&request={request}");
        }

        public async Task<HttpResponseMessage> DeleteRequest(int simulationId, int userId, string request)
        {
            var client = await GetSimulationClient(simulationId);
            if (client == null) throw new Exception();
            return await client.GetAsync($"/api/Simulation/DeleteRequest?simulationId={simulationId}&userId={userId}&request={request}");
        }

        public async Task<Simulation> GetSimulation(int id)
        {
            var client = await GetSimulationClient(id);
            if (client == null) return null;
            return await client.GetFromJsonAsync<Simulation>($"/api/Simulation/GetSimulation?id={id}");
        }

        public async Task<SimulationAuth> GetMyAuth(int id)
        {
            var client = await GetSimulationClient(id);
            if (client == null) return null;
            return await client.GetFromJsonAsync<SimulationAuth>($"/api/Simulation/GetSimulationAuth?id={id}");
        }

        public async Task<List<SimulationPreset>> GetPresets()
        {
            var client = this._httpService.HttpClient;
            return await client.GetFromJsonAsync<List<SimulationPreset>>("/api/Simulation/GetPresets");
        }

        public async Task<List<SimulationRole>> GetRoles(int id)
        {
            var client = await GetSimulationClient(id);
            if (client == null) return null;
            return await client.GetFromJsonAsync<List<SimulationRole>>($"/api/Simulation/GetSimulationRoles?id={id}");
        }

        public async Task ApplyPreset(int simulationId, string presetId)
        {
            var client = await GetSimulationClient(simulationId);
            if (client == null) throw new Exception();
            await client.GetAsync($"/api/Simulation/ApplyPreset?simulationId={simulationId}&presetId={presetId}");
        }

        private async Task<HttpClient> GetSimulationClient(int id)
        {
            var token = await GetSimulationToken(id);
            if (token == null) return null;
            var client = this._httpService.HttpClient;
            if (client.DefaultRequestHeaders.Contains("simsimtoken"))
            {
                // Its easier to just remove this header element and create a newone than
                // it is to search of there is more than one value behind it.
                client.DefaultRequestHeaders.Remove("simsimtoken");
            }

            client.DefaultRequestHeaders.Add("simsimtoken", token.Token);
            return client;
        }

        public async Task StoreToken(SimulationToken token)
        {
            if (token == null)
                throw new ArgumentException("You have to pass a token here!");
            var tokens = await this._localStorage.GetItemAsync<ICollection<SimulationToken>>("munity_simsims");
            if (tokens == null)
                tokens = new List<SimulationToken>();

            SimulationToken element = null;  
            if (tokens.Any())
                element = tokens.FirstOrDefault(n => n.SimulationId == token.SimulationId);
            if (element != null)
            {
                element.Name = token.Name;
                element.Token = token.Token;
            }
            else
            {
                tokens.Add(token);
            }
            await this._localStorage.SetItemAsync("munity_simsims", tokens);
        }

        public async Task<SocketHandlers.SimulationSocketHandler> Subscribe(int simulationId)
        {
            var socket = await SocketHandlers.SimulationSocketHandler.CreateHander();
            var connId = socket.HubConnection.ConnectionId;
            var client = await GetSimulationClient(simulationId);
            var result = await client.GetAsync($"/api/Simulation/Subscribe?id={simulationId}&connectionId={connId}");
            if (result.IsSuccessStatusCode)
                return socket;
            return null;
        }

        public SimulationService(HttpService httpService, ILocalStorageService localStorage)
        {
            this._httpService = httpService;
            this._localStorage = localStorage;
        }
    }
}
