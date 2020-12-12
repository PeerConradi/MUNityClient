using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MUNityClient.Extensions;

namespace MUNityClient.Models.Resolution
{
    public class Resolution : IResolution
    {
        public delegate void OnResolutionChanged();

        public event OnResolutionChanged ResolutionChanged;

        public string ResolutionId { get; set; }

        public DateTime Date { get; set; }

        public ResolutionHeader Header { get; set; }

        public ResolutionPreamble Preamble { get; set; }
        public OperativeSection OperativeSection { get; set; }

        public void InvokeChange()
        {
            this.ResolutionChanged?.Invoke();
        }

        
        public Resolution()
        {
            ResolutionId = Guid.NewGuid().ToString();
            Preamble = new ResolutionPreamble();
            OperativeSection = new OperativeSection();
            Header = new ResolutionHeader();
        }
    }
}
