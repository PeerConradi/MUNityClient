using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MUNityClient.Models.Resolution
{
    public interface IAddAmendment : IAmendment
    {
        public string ParentSectionId { get; set; }

        public int Position { get; set; }

        public string Text { get; set; }
    }
}
