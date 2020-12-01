using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MUNityClient.Models.Resolution
{
    public class OperativeParagraph : IOperativeParagraph
    {
        public string OperativeParagraphId { get; set; }
        public string Name { get; set; } = "";
        public bool IsLocked { get; set; } = false;
        public bool IsVirtual { get; set; } = false;
        public string Text { get; set; } = "";
        public bool Visible { get; set; } = true;

        public List<OperativeParagraph> Children { get; set; }

        public List<Notice> Notices { get; set; }

        public OperativeParagraph()
        {
            Children = new List<OperativeParagraph>();
            OperativeParagraphId = Guid.NewGuid().ToString();
            Notices = new List<Notice>();
        }
    }
}
