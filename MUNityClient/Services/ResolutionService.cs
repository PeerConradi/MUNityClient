﻿using System;
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

        private readonly HttpClient _httpClient;

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
                var result = await this._httpClient.GetAsync($"/api/Resolution/IsUp");
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

        /// <summary>
        /// This is needed because when the model comes without/or with an empty Paragraph list it is set to null
        /// and not to an empty list/array.
        /// </summary>
        /// <param name="resolution"></param>
        private void FixResolution(Resolution resolution)
        {
            if (resolution.Header == null) resolution.Header = new ResolutionHeader();
            if (resolution.Preamble == null) resolution.Preamble = new ResolutionPreamble();
            if (resolution.Preamble.Paragraphs == null) resolution.Preamble.Paragraphs = new List<PreambleParagraph>();
            if (resolution.OperativeSection == null) resolution.OperativeSection = new OperativeSection();
            if (resolution.OperativeSection.Paragraphs == null) resolution.OperativeSection.Paragraphs = new List<OperativeParagraph>();
            if (resolution.OperativeSection.DeleteAmendments == null) resolution.OperativeSection.DeleteAmendments = new List<DeleteAmendment>();
            if (resolution.OperativeSection.AddAmendments == null) resolution.OperativeSection.AddAmendments = new List<AddAmendment>();
            if (resolution.OperativeSection.MoveAmendments == null) resolution.OperativeSection.MoveAmendments = new List<MoveAmendment>();
            if (resolution.OperativeSection.ChangeAmendments == null) resolution.OperativeSection.ChangeAmendments = new List<ChangeAmendment>();
            resolution.OperativeSection.Paragraphs.RemoveAll(n => n == null);
            resolution.Preamble.Paragraphs.RemoveAll(n => n == null);
            resolution.OperativeSection.AddAmendments.RemoveAll(n => n == null);
            resolution.OperativeSection.ChangeAmendments.RemoveAll(n => n == null);
            resolution.OperativeSection.DeleteAmendments.RemoveAll(n => n == null);
            resolution.OperativeSection.MoveAmendments.RemoveAll(n => n == null);
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

        public Task<HttpResponseMessage> UpdatePublicResolutionPreambleParagraph(string resolutionid, PreambleParagraph paragraph)
        {
            var content = JsonContent.Create(paragraph);
            return this._httpClient.PatchAsync($"/api/Resolution/UpdatePublicResolutionPreambleParagraph?resolutionid={resolutionid}", content);
        }

        public async void SaveOfflineResolution(Resolution resolution)
        {
            await this.StoreResolution(resolution);
        }

        public async Task<HubConnection> Subscribe(Resolution resolution)
        {
            var hub = new HubConnectionBuilder().WithUrl($"{Program.API_URL}/resasocket").Build();

            hub.On<Resolution>("ResolutionChanged", (newREsolution) =>
            {
                if (resolution.ResolutionId == newREsolution.ResolutionId)
                {
                    resolution.Preamble = newREsolution.Preamble ?? new ResolutionPreamble();
                    if (resolution.OperativeSection != null && newREsolution.OperativeSection != null)
                    {
                        Console.WriteLine("Operative Paragraph changed");
                        resolution.OperativeSection.Paragraphs = newREsolution.OperativeSection.Paragraphs ?? new List<OperativeParagraph>();
                    }
                    resolution.Header = newREsolution.Header ?? new ResolutionHeader();
                }
            });

            hub.On<string, PreambleParagraph>("PreambleParagraphChanged", (resolutionId, paragraph) =>
            {
                if (resolutionId == resolution.ResolutionId)
                {
                    var targetParagraph = resolution.Preamble.Paragraphs.FirstOrDefault(n => n.PreambleParagraphId == paragraph.PreambleParagraphId);
                    if (targetParagraph != null)
                    {
                        targetParagraph.Text = paragraph.Text;
                        Console.WriteLine("Change Text!");
                    }
                }
            });

            await hub.StartAsync();
            await this._httpClient.GetAsync($"/api/Resolution/SubscribeToResolution?resolutionid={resolution.ResolutionId}&connectionid={hub.ConnectionId}");
            return hub;
        }

        private async Task SocketResolutionChanged(Resolution targetResolution, Resolution resolution)
        {
            if (resolution.ResolutionId != targetResolution.ResolutionId) return;
            
            await StoreResolution(targetResolution);
            Console.WriteLine("Socket updated this resolution!");
        }

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


        public ResolutionService(HttpClient client, ILocalStorageService localStorage)
        {
            this._httpClient = client;
            this._localStorage = localStorage;
        }
    }
}
