using MUNityClient.Models.Simulation;
using MUNityClient.Models.Simulation.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http.Json;
using Blazored.LocalStorage;

namespace MUNityClient.Services
{
    public class SimulationService
    {
        private readonly HttpService _httpService;

        private readonly ILocalStorageService _localStorage;

        /// <summary>
        /// Creates a Simulation and returns its token. The token will also be stored inside the 
        /// munity_simsims local storage.
        /// </summary>
        /// <param name="schema"></param>
        /// <returns></returns>
        public async Task<SimulationToken> CreateSimulation(CreateSchema schema)
        {
            var content = JsonContent.Create(schema);
            var result = await this._httpService.HttpClient.PostAsync($"/api/Simulation/CreateSimulation", content);
            if (result.IsSuccessStatusCode)
            {
                var token = await result.Content.ReadFromJsonAsync<SimulationToken>();
                await StoreToken(token);
                return token;
            }
            throw new Exception("something went wrong");
        }

        public async Task<ICollection<SimulationToken>> GetStoredTokens()
        {
            return await this._localStorage.GetItemAsync<ICollection<SimulationToken>>("munity_simsims");
        }

        public async Task<SimulationToken> GetSimulationToken(int id)
        {
            var tokens = await GetStoredTokens();
            return tokens.FirstOrDefault(n => n.SimulationId == id);
        }

        public async Task<Simulation> JoinSimulation(int simulationId, string token)
        {
            throw new NotImplementedException();
        }

        public async Task<Simulation> GetSimulation(int id)
        {
            var token = await GetSimulationToken(id);
            if (token == null) return null;
            var client = this._httpService.HttpClient;
            client.DefaultRequestHeaders.Add("simsimtoken", token.Token);
            return await client.GetFromJsonAsync<Simulation>($"/api/Simulation/GetSimulation?id={id}");
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

        public SimulationService(HttpService httpService, ILocalStorageService localStorage)
        {
            this._httpService = httpService;
            this._localStorage = localStorage;
        }
    }
}
