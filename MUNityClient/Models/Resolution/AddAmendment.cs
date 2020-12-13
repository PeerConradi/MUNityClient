using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MUNityClient.Extensions.ResolutionExtensions;

namespace MUNityClient.Models.Resolution
{
    /// <summary>
    /// The Add Amendment is for any operative paragraph that should be added while working on the document.
    /// You can show and hide it. For normal this will reference an Operative Paragraph that is set to
    /// Virutal to be differed from normal operative paragraphs.
    /// </summary>
    public class AddAmendment : IAddAmendment
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string TargetSectionId { get; set; }
        public bool Activated { get; set; }
        public string SubmitterName { get; set; }
        public DateTime SubmitTime { get; set; }
        public string Type { get; set; }

        public string Text { get; set; }

        public bool Apply(OperativeSection parentSection)
        {
            var targetParagraph = parentSection.FindOperativeParagraph(this.TargetSectionId);
            if (targetParagraph == null)
                return false;

            targetParagraph.IsVirtual = false;
            targetParagraph.Visible = true;
            parentSection.RemoveAmendment(this);
            return true;
        }

        public bool Deny(OperativeSection parentResolution)
        {
            parentResolution.RemoveAmendment(this);
            return true;
        }
    }
}
