using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MUNityClient.Models.Resolution
{
    public class PreambleParagraph : IPreambleParagraph
    {
        public string PreambleParagraphId { get; set; }
        public string Text { get; set; }
        public List<Notice> Notices { get; set; }

        public PreambleParagraph()
        {
            PreambleParagraphId = Guid.NewGuid().ToString();
            Notices = new List<Notice>();
        }
    }
}
