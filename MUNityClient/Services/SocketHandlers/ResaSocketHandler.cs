using Microsoft.AspNetCore.SignalR.Client;
using MUNityClient.Models.Resolution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MUNityClient.Services.SocketHandlers
{
    public class ResaSocketHandler
    {
        public delegate void OnResolutionChanged(Resolution resolution);

        public event OnResolutionChanged ResolutionChanged;

        public HubConnection HubConnection { get; set; }

        public List<string> IgnoreTransactions { get; set; }

        private Resolution _resolution;

        private ResaSocketHandler(Resolution resolution)
        {
            _resolution = resolution;
            IgnoreTransactions = new List<string>();

            HubConnection = new HubConnectionBuilder().WithUrl($"{Program.API_URL}/resasocket").Build();

            HubConnection.On<Resolution>("ResolutionChanged", (newResolution) => SocketResolutionChanged(newResolution));
            HubConnection.On<string, PreambleParagraph, string>("PreambleParagraphChanged", (resolutionId, paragraph, tan) => SocketPreambleParagraphChanged(resolutionId, paragraph, tan));
            HubConnection.On<string, OperativeParagraph, string>("OperativeParagraphChanged", (resolutionId, paragraph, tan) => SocketOperativeParagraphChanged(resolutionId, paragraph, tan));
        }

        public static async Task<ResaSocketHandler> CreateHandler(Resolution resolution)
        {
            var instance = new ResaSocketHandler(resolution);
            await instance.HubConnection.StartAsync();
            return instance;
        }

        private void SocketResolutionChanged(Resolution newResolution)
        {
            if (newResolution.ResolutionId != _resolution.ResolutionId) return;

            _resolution.Header = newResolution.Header ?? _resolution.Header;
            _resolution.Preamble = newResolution.Preamble ?? _resolution.Preamble;
            _resolution.OperativeSection = newResolution.OperativeSection ?? _resolution.OperativeSection;
            
            ResolutionChanged?.Invoke(this._resolution);
        }

        private void SocketPreambleParagraphChanged(string resolutionId, PreambleParagraph newParagraph, string tan)
        {
            if (IgnoreTransactions.Any(n => n == tan))
            {
                IgnoreTransactions.Remove(tan);
                return;
            }

            if (resolutionId != _resolution.ResolutionId) return;
            var targetParagraph = _resolution.Preamble.Paragraphs.FirstOrDefault(n => n.PreambleParagraphId == newParagraph.PreambleParagraphId);
            if (targetParagraph != null)
            {
                targetParagraph.Text = newParagraph.Text;
                targetParagraph.Notices = newParagraph.Notices;
            }
            ResolutionChanged?.Invoke(this._resolution);
        }

        private void SocketOperativeParagraphChanged(string resolutionId, OperativeParagraph changedParagraph, string tan)
        {
            if (IgnoreTransactions.Any(n => n == tan))
            {
                IgnoreTransactions.Remove(tan);
                return;
            }

            if (resolutionId != _resolution.ResolutionId) return;
            var paragraph = _resolution.OperativeSection.Paragraphs.FirstOrDefault(n => n.OperativeParagraphId == changedParagraph.OperativeParagraphId);
            if (paragraph == null) return;
            paragraph.Text = changedParagraph.Text;
            paragraph.Notices = changedParagraph.Notices;
            ResolutionChanged?.Invoke(this._resolution);
        }
    }
}
