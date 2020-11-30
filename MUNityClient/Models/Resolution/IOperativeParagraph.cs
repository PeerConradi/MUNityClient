using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MUNityClient.Models.Resolution
{
    public interface IOperativeParagraph
    {
        string OperativeParagraphId { get; set; }

        string Name { get; set; }

        bool IsLocked { get; set; }

        bool IsVirtual { get; set; }

        string Text { get; set; }

        bool Visible { get; set; }
    }
}
