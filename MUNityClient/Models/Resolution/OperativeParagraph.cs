using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MUNityClient.Models.Resolution
{
    public class OperativeParagraph : IOperativeParagraph
    {
        public delegate void OnTextChanged(OperativeParagraph paragraph, string newText, string oldText);

        public event OnTextChanged TextChanged;

        public string OperativeParagraphId { get; set; }
        public string Name { get; set; } = "";
        public bool IsLocked { get; set; } = false;

        /// <summary>
        /// Virtual is true when the Operative Paragraph comes from an AddAmendment and doesn't really count as an
        /// paragraph or if it is from a move amendment and is the paragraph where the orignal should be moved to.
        /// </summary>
        public bool IsVirtual { get; set; } = false;

        private string _text;
        public string Text
        {
            get => this._text;
            set
            {
                if (_text != value) return;
                var oldText = this._text;
                this._text = value;
                this.TextChanged?.Invoke(this, value, oldText);
            }
        }
        public bool Visible { get; set; } = true;
        public bool Corrected { get; set; }//ADDED ~Felix

        public List<OperativeParagraph> Children { get; set; }

        public List<Notice> Notices { get; set; }

        public OperativeParagraph()
        {
            Children = new List<OperativeParagraph>();
            OperativeParagraphId = Guid.NewGuid().ToString();
            Notices = new List<Notice>();
        }
    }
}
