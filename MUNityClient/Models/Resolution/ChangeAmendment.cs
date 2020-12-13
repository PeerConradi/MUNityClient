using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MUNityClient.Extensions.ResolutionExtensions;

namespace MUNityClient.Models.Resolution
{
    /// <summary>
    /// The Change amendment is for changing the text of an operative paragraph.
    /// The amendment contains a value of NewText that contains the whole new Text.
    /// </summary>
    public class ChangeAmendment : IChangeAmendment
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string TargetSectionId { get; set; }
        public bool Activated { get; set; }
        public string SubmitterName { get; set; }
        public DateTime SubmitTime { get; set; }
        public string Type { get; set; }
        public string NewText { get; set; }

        public bool Apply(OperativeSection parentSection)
        {
            parentSection.ChangeAmendments.Remove(this);
            var target = parentSection.FindOperativeParagraph(this.TargetSectionId);
            if (target == null) return false;
            target.Text = this.NewText;
            return true;
        }

        public bool Deny(OperativeSection parentSection)
        {
            parentSection.ChangeAmendments.Remove(this);
            return true;
        }
    }
}
