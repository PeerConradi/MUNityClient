using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MUNityClient.Models.Resolution
{
    public class ResolutionPreamble : IPreamble
    {
        public string PreambleId { get; set; }
        public List<PreambleParagraph> Paragraphs { get; set; }

        public ResolutionPreamble()
        {
            PreambleId = Guid.NewGuid().ToString();
            Paragraphs = new List<PreambleParagraph>();
        }
    }
}
