using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MUNityClient.Models.Resolution;

namespace MUNityClient.Mocking
{
    public class Resolution
    {
        public static Models.Resolution.Resolution CreateTestResolution()
        {
            var resolution = new Models.Resolution.Resolution();
            resolution.ResolutionId = "test";

            resolution.Header.Name = "Test Resolution";
            resolution.Header.AgendaItem = "1A";
            resolution.Header.CommitteeName = "General Assembly";
            resolution.Header.Session = "S 1.3";
            resolution.Header.SubmitterName = "Germany";
            resolution.Header.Supporters = new List<string>();
            resolution.Header.Supporters.Add("France");
            resolution.Header.Supporters.Add("Greece");
            resolution.Header.Topic = "Test Resolution Title";

            resolution.Preamble.Paragraphs.Add(new PreambleParagraph()
            {
                PreambleParagraphId = "preamble01",
                Notices = new List<Models.Resolution.Notice>(),
                Text = "I am the first preamble paragraph "
            });

            resolution.Preamble.Paragraphs.Add(new PreambleParagraph()
            {
                PreambleParagraphId = "preamble02",
                Notices = new List<Models.Resolution.Notice>(),
                Text = "I am another preamble paragraph"
            });

            resolution.OperativeSection.Paragraphs.Add(new OperativeParagraph()
            {
                OperativeParagraphId = "operativeparagraph01",
                Children = new List<OperativeParagraph>(),
                IsLocked = false,
                IsVirtual = false,
                Name = "1",
                Notices = new List<Notice>(),
                Text = "I am the first operative paragraph and on me is an amendment to delete me :(",
                Visible = true
            });



            resolution.OperativeSection.DeleteAmendments.Add(new DeleteAmendment()
            {
                Activated = false,
                Id = "deleteAmendment01",
                Name = "delete paragraph one",
                SubmitterName = "United Kingdom",
                SubmitTime = new DateTime(2020, 12, 24, 12, 0, 0),
                TargetSectionId = "operativeparagraph01",
                Type = "delete"
            });

            return resolution;
        }
    }
}
