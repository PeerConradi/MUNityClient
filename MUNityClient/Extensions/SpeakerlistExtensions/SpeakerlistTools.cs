using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MUNityClient.Models.ListOfSpeakers;

namespace MUNityClient.Extensions.SpeakerlistExtensions
{
    public static class SpeakerlistTools
    {
        public static void AddSpeaker(this ListOfSpeakers listOfSpeakers, Speaker speaker)
        {
            listOfSpeakers.Speakers.Add(speaker);
        }
    }
}
