using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        public int Position { get; set; }

        public bool Apply(Resolution parentResolution)
        {
            throw new NotImplementedException();
        }

        public bool Deny(Resolution parentResolution)
        {
            throw new NotImplementedException();
        }
    }
}
