using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MUNityClient.Models.Resolution
{
    public class Resolution : IResolution
    {

        public string ResolutionId { get; set; }

        public DateTime Date { get; set; }

        public ResolutionHeader Header { get; set; }

        public ResolutionPreamble Preamble { get; set; }
        public OperativeSection OperativeSection { get; set; }

        public OperativeParagraph FindOperativeParagraph(string id)
        {
            foreach(var paragraph in OperativeSection.Paragraphs)
            {
                var result = FindOperativeParagraphRecursive(paragraph, id);
                if (result != null) return result;
            }
            return null;
        }

        private OperativeParagraph FindOperativeParagraphRecursive(OperativeParagraph paragraph, string targetId)
        {
            if (paragraph.OperativeParagraphId == targetId) return paragraph;
            if (paragraph.Children != null && paragraph.Children.Any())
            {
                foreach(var child in paragraph.Children)
                {
                    var result = FindOperativeParagraphRecursive(child, targetId);
                    if (result != null) return result;
                }
            }
            return null;
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
