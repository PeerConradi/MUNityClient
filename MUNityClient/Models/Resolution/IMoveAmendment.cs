using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MUNityClient.Models.Resolution
{
    public interface IMoveAmendment : IAmendment
    {
        public string NewTargetSectionId { get; set; }

        public int Position { get; set; }
    }
}
