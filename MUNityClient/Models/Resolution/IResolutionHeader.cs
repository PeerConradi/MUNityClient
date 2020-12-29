using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MUNityClient.Models.Resolution
{
    public interface IResolutionHeader
    {
        string Name { get; set; }

        string FullName { get; set; }

        string Topic { get; set; }

        public string AgendaItem { get; set; }

        public string Session { get; set; }

        public string SubmitterName { get; set; }

        public string CommitteeName { get; set; }

        public List<string> Supporters { get; set; }
    }
}
