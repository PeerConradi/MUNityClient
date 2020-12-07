using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using MUNityClient.Models.Resolution;

namespace MUNityClient.Services
{
    public class ResolutionService
    {
        public List<Resolution> Resolutions { get; set; }

        private readonly HttpClient _httpClient;

        HubConnection hubConnection;

        public Resolution GetResolutionOfOperativeParagraph(OperativeParagraph paragraph)
        {
            return this.Resolutions?.FirstOrDefault(n => n.OperativeSection.Paragraphs.Contains(paragraph));
        }

        public Resolution GetResolutionOfOperativeParagraph(string id)
        {
            return this.Resolutions?.FirstOrDefault(n => n.OperativeSection.Paragraphs.Any(a => a.OperativeParagraphId == id));
        }

        public Resolution GetResolutionOfPreambleParagraph(PreambleParagraph paragraph)
        {
            return this.Resolutions?.FirstOrDefault(n => n.Preamble.Paragraphs.Contains(paragraph));
        }

        public Resolution GetResolutionOfPreambleParagraph(string id)
        {
            return this.Resolutions?.FirstOrDefault(n => n.Preamble.Paragraphs.Any(a => a.PreambleParagraphId == id));
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
            this.Resolutions.Add(resolution);
            return resolution;
        }

        public async Task<Resolution> GetPublicResolution(string id)
        {
            if (id == "test")
            {
                var testResolution = Mocking.Resolution.CreateTestResolution();
                this.Resolutions.Add(testResolution);
                return testResolution;
            }

            var resolution = await this._httpClient.GetFromJsonAsync<Resolution>($"/api/Resolution/GetPublic?id={id}");
            if (resolution == null)
            {
                Console.WriteLine($"Resolution with the id {id} not found!");
                return null;
            }

            FixResolution(resolution);

            this.Resolutions.Add(resolution);
            return resolution;
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

        private void SocketResolutionChanged(Resolution resolution)
        {
            var resolutionInService = this.Resolutions.FirstOrDefault(n => n.ResolutionId == resolution.ResolutionId);
            if (resolutionInService == null)
                return;

            resolutionInService.Preamble = resolution.Preamble;
            resolutionInService.OperativeSection = resolution.OperativeSection;
            resolutionInService.Header = resolution.Header;
        }


        public ResolutionService(HttpClient client)
        {
            this._httpClient = client;
            this.Resolutions = new List<Resolution>();
        }
    }
}
