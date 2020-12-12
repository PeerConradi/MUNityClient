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

        public bool Apply(Resolution resolution)
        {
            var paragraph = resolution.FindOperativeParagraph(this.TargetSectionId);

            if (resolution?.OperativeSection == null)
                return false;

            if (!resolution.OperativeSection.Paragraphs.Contains(paragraph))
                return false;

            resolution.OperativeSection?.Paragraphs.Remove(paragraph);
            resolution.AmendmentsForOperativeParagraph(this.TargetSectionId).ForEach(n => resolution.RemoveAmendment(n));
            return true;
        }

        public bool Deny(Resolution resolution)
        {
            if (resolution.OperativeSection == null)
                return false;

            var count = resolution.OperativeSection.DeleteAmendments.RemoveAll(n =>
                n.TargetSectionId == this.TargetSectionId);

            if (count > 0)
                return true;

            return false;
        }

        public DeleteAmendment()
        {
            this.SubmitTime = DateTime.Now;
            this.Id = Guid.NewGuid().ToString();
        }
    }
}
