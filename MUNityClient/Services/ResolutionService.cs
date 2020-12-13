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

        private readonly HttpService _httpService;

        private bool? _isOnline = null;

        private DateTime? _lastOnlineChecked;

        /// <summary>
        /// Checks if the Resolution Controller of the API is available.
        /// Will store the value for 30 Seconds and refresh when called 30 seconds after
        /// the last call. You can also set forceRefresh to true to force a new IsUp call to the API
        /// </summary>
        /// <returns></returns>
        public async Task<bool> IsOnline(bool forceRefresh = false)
        {
            if (forceRefresh || _isOnline == null || _lastOnlineChecked == null || (DateTime.Now - _lastOnlineChecked.Value).TotalSeconds > 30)
            {
                var result = await this._httpService.HttpClient.GetAsync($"/api/Resolution/IsUp");
                _isOnline = result.IsSuccessStatusCode;
                _lastOnlineChecked = DateTime.Now;
            }
            return _isOnline.Value;
        }

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
            var resolution = await this._httpService.HttpClient.GetFromJsonAsync<Resolution>($"/api/Resolution/CreatePublic?title={title}");
            if (resolution == null)
                return null;
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
                var resolution = await this._httpService.HttpClient.GetFromJsonAsync<Resolution>($"/api/Resolution/GetPublic?id={id}");
                if (resolution != null)
                {
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

        /// <summary>
        /// Updates a resolution and informs all connected WebSockets
        /// </summary>
        /// <param name="resolution"></param>
        /// <returns></returns>
        public Task<HttpResponseMessage> UpdatePublicResolution(Resolution resolution)
        {
            HttpContent content = JsonContent.Create(resolution);
            return this._httpService.HttpClient.PatchAsync($"/api/Resolution/UpdatePublicResolution", content);
        }

        public Task<HttpResponseMessage> UpdatePublicResolutionPreambleParagraph(string resolutionid, PreambleParagraph paragraph)
        {
            var content = JsonContent.Create(paragraph);
            return this._httpService.HttpClient.PatchAsync($"/api/Resolution/UpdatePublicResolutionPreambleParagraph?resolutionid={resolutionid}", content);
        }

        public Task<HttpResponseMessage> UpdatePublicResolutionOperativeParagraph(string resolutionid, OperativeParagraph paragraph)
        {
            var content = JsonContent.Create(paragraph);
            return this._httpService.HttpClient.PatchAsync($"/api/Resolution/UpdatePublicResolutionOperativeParagraph?resolutionid={resolutionid}", content);
        }

        public async void SaveOfflineResolution(Resolution resolution)
        {
            await this.StoreResolution(resolution);
        }

        #region SignalR WebSocket

        public async Task<HubConnection> Subscribe(Resolution resolution)
        {
            var hub = new HubConnectionBuilder().WithUrl($"{Program.API_URL}/resasocket").Build();

            hub.On<Resolution>("ResolutionChanged", (newResolution) => SocketResolutionChanged(resolution, newResolution));
            hub.On<string, PreambleParagraph>("PreambleParagraphChanged", (resolutionId, paragraph) => SocketPreambleParagraphChanged(resolution, resolutionId, paragraph));
            hub.On<string, OperativeParagraph>("OperativeParagraphChanged", (resolutionId, paragraph) => SocketOperativeParagraphChanged(resolution, resolutionId, paragraph));

            await hub.StartAsync();
            await this._httpService.HttpClient.GetAsync($"/api/Resolution/SubscribeToResolution?resolutionid={resolution.ResolutionId}&connectionid={hub.ConnectionId}");
            return hub;
        }


        private async Task SocketResolutionChanged(Resolution targetResolution, Resolution newResolution)
        {
            if (newResolution.ResolutionId != targetResolution.ResolutionId) return;

            targetResolution.Preamble = newResolution.Preamble ?? new ResolutionPreamble();
            if (targetResolution.OperativeSection != null && newResolution.OperativeSection != null)
            {
                Console.WriteLine("Operative Paragraph changed");
                targetResolution.OperativeSection.Paragraphs = newResolution.OperativeSection.Paragraphs ?? new List<OperativeParagraph>();
            }
            targetResolution.Header = newResolution.Header ?? new ResolutionHeader();

            await StoreResolution(targetResolution);
        }

        private void SocketPreambleParagraphChanged(Resolution targetResolution, string resolutionId, PreambleParagraph newParagraph)
        {
            if (resolutionId == targetResolution.ResolutionId)
            {
                var targetParagraph = targetResolution.Preamble.Paragraphs.FirstOrDefault(n => n.PreambleParagraphId == newParagraph.PreambleParagraphId);
                if (targetParagraph != null)
                {
                    targetParagraph.Text = newParagraph.Text;
                    targetParagraph.Notices = newParagraph.Notices;
                }
            }
        }

        private void SocketOperativeParagraphChanged(Resolution targetREsolution, string resolutionId, OperativeParagraph changedParagraph)
        {
            if (resolutionId != targetREsolution.ResolutionId) return;
            var paragraph = targetREsolution.OperativeSection.Paragraphs.FirstOrDefault(n => n.OperativeParagraphId == changedParagraph.OperativeParagraphId);
            if (paragraph == null) return;
            paragraph.Text = changedParagraph.Text;
            paragraph.Notices = changedParagraph.Notices;
        }

        #endregion

        public bool HasValidOperator(PreambleParagraph paragraph)
        {
            // TODO
            return false;
        }

        public bool HasValidOperator(OperativeParagraph paragraph)
        {
            // TODO
            return false;
        }


        public ResolutionService(HttpService client, ILocalStorageService localStorage)
        {
            this._httpService = client;
            this._localStorage = localStorage;
        }

    }
}
