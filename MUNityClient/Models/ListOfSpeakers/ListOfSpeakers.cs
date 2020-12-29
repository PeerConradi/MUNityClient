using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MUNityClient.Models.ListOfSpeakers
{
    public class ListOfSpeakers
    {
        public delegate void OnListChanged();

        public event OnListChanged ListChanged;

        public const string newListName = "Neue Redeliste";

        public enum EStatus
        {
            Stopped,
            Speaking,
            Question,
            Answer,
            SpeakerPaused,
            QuestionPaused,
            AnswerPaused
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

                // Fall für das Fortsetzen eienr Antwort!
                var finishTimeQuestion = StartSpeakerTime.AddSeconds(QuestionTime.TotalSeconds);
                return finishTimeQuestion - DateTime.Now;
            }
        }

        public TimeSpan RemainingQuestionTime
        {
            get
            {
                if (Status != EStatus.Question && Status != EStatus.QuestionPaused) return PausedQuestionTime;

                var finishTime = StartQuestionTime.AddSeconds(QuestionTime.TotalSeconds);
                return finishTime - DateTime.Now;
            }
        }

        public List<Speaker> Speakers { get; set; }

        public List<Speaker> Questions { get; set; }

        public Speaker CurrentSpeaker { get; set; }

        public Speaker CurrentQuestion { get; set; }


        public bool ListClosed { get; set; } = false;

        public bool QuestionsClosed { get; set; } = false;

        public TimeSpan LowTimeMark { get; set; }

        public DateTime StartSpeakerTime { get; set; }

        public DateTime StartQuestionTime { get; set; }

        public void NextSpeaker()
        {
            if (Speakers.Any())
            {
                CurrentSpeaker = Speakers.First();
                Speakers.Remove(Speakers.First());
            }
            this.Status = EStatus.Stopped;
            this.ListChanged?.Invoke();
        }

        public void NextQuestion()
        {
            if (Questions.Any())
            {
                CurrentQuestion = Questions.First();
                Questions.Remove(Questions.First());
            }
            this.Status = EStatus.Stopped;
            this.ListChanged?.Invoke();
        }

        public void StartSpeaker()
        {
            if (CurrentSpeaker != null)
            {
                this.PausedQuestionTime = QuestionTime;
                this.StartSpeakerTime = DateTime.Now;
                this.Status = EStatus.Speaking;
            }
            else
            {
                this.Status = EStatus.Stopped;
            }
            this.ListChanged?.Invoke();
        }

        public void StartQuestion()
        {
            if (this.CurrentQuestion != null)
            {
                this.PausedSpeakerTime = SpeakerTime;
                this.StartQuestionTime = DateTime.Now;
                this.Status = EStatus.Question;
            }
            else
            {
                this.Status = EStatus.Stopped;
            }
            this.ListChanged?.Invoke();
        }

        public void PauseSpeaker()
        {
            this.PausedSpeakerTime = RemainingSpeakerTime;
            if (Status == EStatus.Speaking)
                this.Status = EStatus.SpeakerPaused;
            else if (Status == EStatus.Answer)
                this.Status = EStatus.AnswerPaused;
            else
                this.Status = EStatus.Stopped;

            this.ListChanged?.Invoke();
        }

        public void PauseQuestion()
        {
            this.PausedQuestionTime = RemainingQuestionTime;
            this.Status = EStatus.QuestionPaused;

            this.ListChanged?.Invoke();
        }

        public void ResumeSpeaker()
        {

            if (CurrentSpeaker != null)
            {
                if (Status == EStatus.SpeakerPaused)
                    this.StartSpeakerTime = DateTime.Now.AddSeconds(RemainingSpeakerTime.TotalSeconds - SpeakerTime.TotalSeconds);
                else if (Status == EStatus.AnswerPaused)
                    this.StartSpeakerTime = DateTime.Now.AddSeconds(RemainingSpeakerTime.TotalSeconds - QuestionTime.TotalSeconds);
                else
                {
                    this.StartSpeakerTime = DateTime.Now;
                }

                // Fixes a small glitch in the Question time!
                this.StartQuestionTime = DateTime.Now;
                this.Status = EStatus.Speaking;
            }
            else
            {
                this.Status = EStatus.Stopped;
            }
            this.ListChanged?.Invoke();
        }

        public void ResumeQuestion()
        {
            if (CurrentQuestion != null)
            {
                this.StartQuestionTime = DateTime.Now.AddSeconds(RemainingQuestionTime.TotalSeconds - QuestionTime.TotalSeconds);
                this.Status = EStatus.Question;
            }
            else
            {
                this.Status = EStatus.Stopped;
            }
            this.ListChanged?.Invoke();
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
            this.ListChanged?.Invoke();
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
            this.ListChanged?.Invoke();
            return newSpeaker;
        }

        public void ClearCurrentSpeaker()
        {
            if (this.Status == EStatus.Speaking || this.Status == EStatus.SpeakerPaused || this.Status == EStatus.Answer || this.Status == EStatus.AnswerPaused)
                this.Status = EStatus.Stopped;
            this.CurrentQuestion = null;
            this.ListChanged?.Invoke();
        }

        public void ClearCurrentQuestion()
        {
            if (this.Status == EStatus.Question || this.Status == EStatus.QuestionPaused)
                this.Status = EStatus.Stopped;
            this.CurrentQuestion = null;
            this.ListChanged?.Invoke();
        }

        public void AddSpeakerSeconds(int seconds)
        {
            this.StartSpeakerTime = this.StartSpeakerTime.AddSeconds(seconds);
            this.ListChanged?.Invoke();
        }

        public void AddQuestionSeconds(int seconds)
        {
            this.StartSpeakerTime = this.StartQuestionTime.AddSeconds(seconds);
            this.ListChanged?.Invoke();
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
