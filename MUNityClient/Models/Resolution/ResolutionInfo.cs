using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MUNityClient.Models.Resolution
{
    public class ResolutionInfo
    {
        public string ResolutionId { get; set; }

        public string Title { get; set; }

        public DateTime LastChangedDate { get; set; }

        public static explicit operator ResolutionInfo(MUNity.Models.Resolution.Resolution resolution)
        {
            return new ResolutionInfo()
            {
                LastChangedDate = resolution.Date,
                ResolutionId = resolution.ResolutionId,
                Title = resolution.Header.Topic
            };
        }
    }
}
