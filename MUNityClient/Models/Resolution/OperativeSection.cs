using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MUNityClient.Models.Resolution
{
    public class OperativeSection : IOperativeSection
    {
        public string OperativeSectionId { get; set; }
        public List<OperativeParagraph> Paragraphs { get; set; }
        public List<ChangeAmendment> ChangeAmendments { get; set; }
        public List<AddAmendment> AddAmendments { get; set; }
        public List<MoveAmendment> MoveAmendments { get; set; }
        public List<DeleteAmendment> DeleteAmendments { get; set; }

        public OperativeSection()
        {
            OperativeSectionId = Guid.NewGuid().ToString();
            Paragraphs = new List<OperativeParagraph>();
            ChangeAmendments = new List<ChangeAmendment>();
            AddAmendments = new List<AddAmendment>();
            MoveAmendments = new List<MoveAmendment>();
            DeleteAmendments = new List<DeleteAmendment>();
        }
    }
}
