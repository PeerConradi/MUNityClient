using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MUNityClient.Models.Resolution;

namespace MUNityClient.Extensions.ResolutionExtensions
{
    public static class ResolutionTools
    {
        public static ResolutionInfo GetInfo(this Resolution resolution)
        {
            var info = new ResolutionInfo()
            {
                LastChangedDate = resolution.Date,
                ResolutionId = resolution.ResolutionId,
                Title = resolution.Header?.Topic
            };
            return info;
        }
    }
}
