using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MUNityClient.Models.Resolution
{
    public interface IPreambleParagraph
    {
        string PreambleParagraphId { get; set; }

        string Text { get; set; }

        List<Notice> Notices { get; set; }
    }
}
