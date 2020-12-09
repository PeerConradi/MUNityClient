using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MUNityClient.Models.Resolution
{
    public class PreambleParagraph : IPreambleParagraph
    {
        public delegate void OnTextChanged(PreambleParagraph sender, string oldText, string newText);

        public event OnTextChanged TextChanged;

        public string PreambleParagraphId { get; set; }

        private string _text = "";
        public string Text
        {
            get => _text;
            set
            {
                
                var oldText = _text;
                if (value != oldText)
                {
                    _text = value;
                    TextChanged?.Invoke(this, oldText, value);
                }
            }
        }
        public bool IsLocked { get; set; } = false;

        public bool Corrected { get; set; }

        public List<Notice> Notices { get; set; }

        public PreambleParagraph()
        {
            PreambleParagraphId = Guid.NewGuid().ToString();
            Notices = new List<Notice>();
        }
    }
}
