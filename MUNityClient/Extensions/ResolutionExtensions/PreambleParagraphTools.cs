using MUNityClient.Models.Resolution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MUNityClient.Extensions.ResolutionExtensions
{
    public static class PreambleParagraphTools
    {
        public static PreambleParagraph CreatePreambleParagraph(this Resolution resolution)
        {
            var paragraph = new PreambleParagraph();
            resolution.Preamble.Paragraphs.Add(paragraph);
            return paragraph;
        }

        public static bool HasValidOperator(this PreambleParagraph paragraph)
        {
            return false;
        }
    }
}
