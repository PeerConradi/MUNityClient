using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using MUNityClient.Models.Resolution;
using Blazored.LocalStorage;

namespace MUNityClient.Services
{
    public class ResolutionService
    {
        private readonly ILocalStorageService _localStorage;


        public async Task<List<Resolution>> GetStoredResolutions()
        {
            var resolutions = await this._localStorage.GetItemAsync<List<Resolution>>("munity_resolutions");
            if (resolutions == null) return new List<Resolution>();
            return resolutions;
        }

        private async Task StoreResolutions()
        {
            var resolutions = await this._localStorage.GetItemAsync<List<Resolution>>("munity_resolutions");
            if (resolutions == null) resolutions = new List<Resolution>();
            await this._localStorage.SetItemAsync("munity_resolutions", resolutions);
        }

        private async Task StoreResolution(Resolution resolution)
        {
            var resolutions = await this._localStorage.GetItemAsync<List<Resolution>>("munity_resolutions");
            if (resolutions == null) resolutions = new List<Resolution>();
            var resolutionClone = resolutions.FirstOrDefault(n => n.ResolutionId == resolution.ResolutionId);

            if (resolutionClone != null)
            {
                // Found a matchin Resolution but its not the same instance
                if (resolutionClone != resolution)
                {
                    // Store all values
                    resolutionClone.Header = resolution.Header;
                    resolutionClone.Preamble = resolution.Preamble;
                    resolutionClone.OperativeSection = resolution.OperativeSection;
                    resolutionClone.Date = resolution.Date;
                }
            }
            else
            {
                resolutions.Add(resolution);
            }
            await this._localStorage.SetItemAsync("munity_resolutions", resolutions);
        }


        private readonly HttpClient _httpClient;

        HubConnection hubConnection;

        public async Task<Resolution> GetResolutionOfOperativeParagraph(OperativeParagraph paragraph)
        {
            var resolutions = await GetStoredResolutions();
            return resolutions.FirstOrDefault(n => n.OperativeSection.Paragraphs.Contains(paragraph));
        }

        public async Task<Resolution> GetResolutionOfOperativeParagraph(string id)
        {
            var resolutions = await GetStoredResolutions();
            return resolutions.FirstOrDefault(n => n.OperativeSection.Paragraphs.Any(a => a.OperativeParagraphId == id));
        }

        public async Task<Resolution> GetResolutionOfPreambleParagraph(PreambleParagraph paragraph)
        {
            var resolutions = await GetStoredResolutions();
            return resolutions.FirstOrDefault(n => n.Preamble.Paragraphs.Contains(paragraph));
        }

        public async Task<Resolution> GetResolutionOfPreambleParagraph(string id)
        {
            var resolutions = await GetStoredResolutions();
            return resolutions.FirstOrDefault(n => n.Preamble.Paragraphs.Any(a => a.PreambleParagraphId == id));
        }

        public async Task<Resolution> CreateOffline()
        {
            var resolution = new Resolution();
            resolution.Header.Topic = "No Title";
            await this.StoreResolution(resolution);
            return resolution;
        }

        /// <summary>
        /// Creates a public resolution and adds it to the editing resolution list.
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        public async Task<Resolution> CreatePublicResolution(string title)
        {
            var resolution = await this._httpClient.GetFromJsonAsync<Resolution>($"/api/Resolution/CreatePublic?title={title}");
            if (resolution == null)
                return null;
            FixResolution(resolution);
            await this.StoreResolution(resolution);
            return resolution;
        }

        public async Task<Resolution> GetOfflineResolution(string id)
        {
            // Look into the local Storage for the resolution
            var resolutionsLocal = await this.GetStoredResolutions();
            return resolutionsLocal.FirstOrDefault(n => n.ResolutionId == id);
        }

        public async Task<Resolution> GetPublicResolution(string id)
        {
            if (id == "test")
            {
                var testResolution = Mocking.Resolution.CreateTestResolution();
                await this.StoreResolution(testResolution);
                return testResolution;
            }

            try
            {
                var resolution = await this._httpClient.GetFromJsonAsync<Resolution>($"/api/Resolution/GetPublic?id={id}");
                if (resolution != null)
                {
                    FixResolution(resolution);

                    await this.StoreResolution(resolution);
                    return resolution;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Unable to reach the server!");
                
            }

            return null;
        }

        private void FixResolution(Resolution resolution)
        {
            if (resolution.Header == null) resolution.Header = new ResolutionHeader();
            if (resolution.Preamble == null) resolution.Preamble = new ResolutionPreamble();
            if (resolution.Preamble.Paragraphs == null) resolution.Preamble.Paragraphs = new List<PreambleParagraph>();
            if (resolution.OperativeSection == null) resolution.OperativeSection = new OperativeSection();
            if (resolution.OperativeSection.Paragraphs == null) resolution.OperativeSection.Paragraphs = new List<OperativeParagraph>();
        }

        /// <summary>
        /// Updates a resolution and informs all connected WebSockets
        /// </summary>
        /// <param name="resolution"></param>
        /// <returns></returns>
        public Task<HttpResponseMessage> UpdatePublicResolution(Resolution resolution)
        {
            HttpContent content = JsonContent.Create(resolution);
            return this._httpClient.PatchAsync($"/api/Resolution/UpdatePublicResolution", content);
        }

        public async void SaveOfflineResolution(Resolution resolution)
        {
            await this.StoreResolution(resolution);
        }

        public async Task Subscribe(Resolution resolution)
        {
            if (hubConnection == null)
            {
                hubConnection = new HubConnectionBuilder().WithUrl($"{Program.API_URL}/resasocket").Build();

                hubConnection.On<Resolution>("ResolutionChanged", SocketResolutionChanged);

                await hubConnection.StartAsync();
            }

            await this._httpClient.GetAsync($"/api/Resolution/SubscribeToResolution?resolutionid={resolution.ResolutionId}&connectionid={hubConnection.ConnectionId}");
        }

        private async Task SocketResolutionChanged(Resolution resolution)
        {
            var resolutions = await this.GetStoredResolutions();
            var resolutionInService = resolutions.FirstOrDefault(n => n.ResolutionId == resolution.ResolutionId);
            if (resolutionInService == null)
                return;

            resolutionInService.Preamble = resolution.Preamble;
            resolutionInService.OperativeSection = resolution.OperativeSection;
            resolutionInService.Header = resolution.Header;
        }


        public ResolutionService(HttpClient client, ILocalStorageService localStorage)
        {
            this._httpClient = client;
            this._localStorage = localStorage;
        }
    }
}
