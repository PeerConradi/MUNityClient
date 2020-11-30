using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MUNityClient.Models.Resolution
{
    public interface IPreamble
    {
        string PreambleId { get; set; }

        List<PreambleParagraph> Paragraphs { get; set; }
    }
}
