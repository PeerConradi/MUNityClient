using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MUNityClient.Models.Resolution
{
    public interface IAmendment
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string TargetSectionId { get; set; }

        public bool Activated { get; set; }

        public string SubmitterName { get; set; }

        public DateTime SubmitTime { get; set; }

        public string Type { get; set; }

        public bool Apply(OperativeSection parentSection);

        public bool Deny(OperativeSection parentSection);

    }
}
