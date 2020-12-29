using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MUNityClient.Extensions.ResolutionExtensions;

namespace MUNityClient.Models.Resolution
{
    public class MoveAmendment : IMoveAmendment
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string TargetSectionId { get; set; }
        public bool Activated { get; set; }
        public string SubmitterName { get; set; }
        public DateTime SubmitTime { get; set; }
        public string Type { get; set; }

        public string NewTargetSectionId { get; set; }

        public bool Apply(OperativeSection parentSection)
        {
            var placeholder = parentSection.FindOperativeParagraph(NewTargetSectionId);
            var target = parentSection.FindOperativeParagraph(TargetSectionId);

            if (target == null || placeholder == null)
                return false;

            placeholder.Children = target.Children;
            placeholder.Corrected = target.Corrected;
            placeholder.IsLocked = false;
            placeholder.IsVirtual = false;
            placeholder.Name = target.Name;
            placeholder.OperativeParagraphId = target.OperativeParagraphId;
            target.OperativeParagraphId = Guid.NewGuid().ToString();
            this.TargetSectionId = target.OperativeParagraphId;
            placeholder.Text = target.Text;
            placeholder.Visible = true;
            parentSection.RemoveOperativeParagraph(target);
            return true;
        }

        public bool Deny(OperativeSection parentSection)
        {
            parentSection.RemoveAmendment(this);
            return true;
        }
    }
}
