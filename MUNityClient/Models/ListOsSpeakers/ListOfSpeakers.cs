using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MUNityClient.Models.ListOsSpeakers
{
    public class ListOfSpeakers
    {
        public const string newListName = "Neue Redeliste";

        public enum EStatus
        {
            Stopped,
            Speaking,
            Question,
            Answer,
            SpeakerPaused,
            QuestionPaused
        }

        public string ListOfSpeakersId { get; set; }

        public string PublicId { get; set; }

        public string Name { get; set; }

        public EStatus Status { get; set; }

        public TimeSpan SpeakerTime { get; set; }

        public TimeSpan QuestionTime { get; set; }

        public TimeSpan PausedSpeakerTime { get; set; }

        public TimeSpan PausedQuestionTime { get; set; }

        public TimeSpan RemainingSpeakerTime
        {
            get
            {
                if (Status == EStatus.Stopped || Status == EStatus.Question || Status == EStatus.SpeakerPaused || Status == EStatus.QuestionPaused)
                {
                    return PausedSpeakerTime;
                }
                else if (Status == EStatus.Speaking)
                {
                    var finishTime = StartSpeakerTime.AddSeconds(SpeakerTime.TotalSeconds);
                    return finishTime - DateTime.Now;
                    // Startzeitpunkt                 Startzeitpunkt + Speakertime
                    //       |---------------|<-------->|
                    //                          Verbleibende Zeit
                }
                var finishTimeQuestion = StartSpeakerTime.AddSeconds(QuestionTime.TotalSeconds);
                return finishTimeQuestion - DateTime.Now;
            }
        }

        public TimeSpan RemainingQuestionTime
        {
            get
            {
                if (Status == EStatus.Stopped || Status == EStatus.Speaking || Status == EStatus.Answer) return PausedQuestionTime;

                var finishTime = StartQuestionTime.AddSeconds(QuestionTime.TotalSeconds);
                return finishTime - DateTime.Now;
            }
        }

        public List<Speaker> Speakers { get; set; }

        public List<Speaker> Questions { get; set; }

        public Speaker CurrentSpeaker { get; set; }

        public Speaker CurrentQuestion { get; set; }

        
        public bool ListClosed { get; set; }

        public bool QuestionsClosed { get; set; }

        public TimeSpan LowTimeMark { get; set; }

        public DateTime StartSpeakerTime { get; set; }

        public DateTime StartQuestionTime { get; set; }

        public void NextSpeaker()
        {
            if (Speakers.Any())
            {
                CurrentSpeaker = Speakers.First();
                Speakers.Remove(Speakers.First());
                this.Status = EStatus.Stopped;
            }
        }

        public void StartSpeaker()
        {
            this.PausedQuestionTime = QuestionTime;
            this.StartSpeakerTime = DateTime.Now;
            this.Status = EStatus.Speaking;
        }

        public void PauseSpeaker()
        {
            this.PausedSpeakerTime = RemainingSpeakerTime;
            this.Status = EStatus.SpeakerPaused;
        }

        public void ResumeSpeaker()
        {
            this.StartSpeakerTime = DateTime.Now.AddSeconds(RemainingSpeakerTime.TotalSeconds - SpeakerTime.TotalSeconds);
            this.Status = EStatus.Speaking;
        }

        public Speaker AddSpeaker(string name, string iso = "")
        {
            var newSpeaker = new Speaker()
            {
                Id = Guid.NewGuid().ToString(),
                Iso = iso,
                Name = name
            };
            Speakers.Add(newSpeaker);
            Questions.Clear();
            return newSpeaker;
        }

        public Speaker AddQuestion(string name, string iso = "")
        {
            var newSpeaker = new Speaker()
            {
                Id = Guid.NewGuid().ToString(),
                Iso = iso,
                Name = name
            };
            Questions.Add(newSpeaker);
            return newSpeaker;
        }

        public ListOfSpeakers()
        {
            this.Speakers = new List<Speaker>();
            this.Questions = new List<Speaker>();
            this.ListOfSpeakersId = Guid.NewGuid().ToString();
            this.SpeakerTime = new TimeSpan(0, 3, 0);
            this.QuestionTime = new TimeSpan(0, 0, 30);
            this.PausedSpeakerTime = this.SpeakerTime;
            this.PausedQuestionTime = this.QuestionTime;
        }
    }
}
