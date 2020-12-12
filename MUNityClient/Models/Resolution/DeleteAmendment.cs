using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MUNityClient.Extensions.ResolutionExtensions;

namespace MUNityClient.Models.Resolution
{
    /// <summary>
    /// The Delete Amendment is for removing an operative paragraph from the resolution.
    /// </summary>
    public class DeleteAmendment : IDeleteAmendment
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string TargetSectionId { get; set; }
        public bool Activated { get; set; }
        public string SubmitterName { get; set; }
        public DateTime SubmitTime { get; set; }
        public string Type { get; set; }

        public bool Apply(OperativeSection parentSection)
        {
            var paragraph = parentSection.FindOperativeParagraph(this.TargetSectionId);

            if (!parentSection.Paragraphs.Contains(paragraph))
                return false;

            parentSection.Paragraphs.Remove(paragraph);

            parentSection.AmendmentsForOperativeParagraph(this.TargetSectionId).ForEach(n => parentSection.RemoveAmendment(n));
            return true;
        }

        public bool Deny(OperativeSection section)
        {
            var count = section.DeleteAmendments.RemoveAll(n =>
                n.TargetSectionId == this.TargetSectionId);

            return count > 0;
        }

        public DeleteAmendment()
        {
            this.SubmitTime = DateTime.Now;
            this.Id = Guid.NewGuid().ToString();
        }
    }
}
