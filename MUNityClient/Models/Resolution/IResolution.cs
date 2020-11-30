using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MUNityClient.Models.Resolution
{
    public interface IResolution
    {

        public string ResolutionId { get; set; }

        public ResolutionHeader Header { get; set; }

        public DateTime Date { get; set; }

        public ResolutionPreamble Preamble { get; set; }

        public OperativeSection OperativeSection { get; set; }

    }
}
