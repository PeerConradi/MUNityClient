using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MUNityClient.Models.Resolution
{
    public interface IChangeAmendment : IAmendment
    {
        public string NewText { get; set; }
    }
}
