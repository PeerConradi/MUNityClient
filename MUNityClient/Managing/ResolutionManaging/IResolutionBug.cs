using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MUNitySchema.Models.Resolution;

namespace MUNityClient.Managing.ResolutionManaging
{
    interface IResolutionBug
    {
        string Description { get; }

        bool Detect(Resolution resolution);

        bool Fix(Resolution resolution);
    }
}
