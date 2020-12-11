using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MUNityClient.Models.Resolution
{
    public class Resolution : IResolution
    {
        public delegate void OnResolutionChanged();

        public event OnResolutionChanged ResolutionChanged;

        public string ResolutionId { get; set; }

        public DateTime Date { get; set; }

        public ResolutionHeader Header { get; set; }

        public ResolutionPreamble Preamble { get; set; }
        public OperativeSection OperativeSection { get; set; }

        public void InvokeChange()
        {
            this.ResolutionChanged?.Invoke();
        }

        public OperativeParagraph FindOperativeParagraph(string id)
        {
            foreach(var paragraph in OperativeSection.Paragraphs)
            {
                var result = FindOperativeParagraphRecursive(paragraph, id);
                if (result != null) return result;
            }
            return null;
        }

        private OperativeParagraph FindOperativeParagraphRecursive(OperativeParagraph paragraph, string targetId)
        {
            if (paragraph.OperativeParagraphId == targetId) return paragraph;
            if (paragraph.Children != null && paragraph.Children.Any())
            {
                foreach(var child in paragraph.Children)
                {
                    var result = FindOperativeParagraphRecursive(child, targetId);
                    if (result != null) return result;
                }
            }
            return null;
        }

        private OperativeParagraph FindOperativeParagraphPathRecursive(OperativeParagraph paragraph, string targetId, List<OperativeParagraph> path)
        {
            if (paragraph.OperativeParagraphId == targetId)
            {
                path.Add(paragraph);
                return paragraph;
            }
            if (paragraph.Children != null && paragraph.Children.Any())
            {
                foreach (var child in paragraph.Children)
                {
                    var result = FindOperativeParagraphPathRecursive(child, targetId, path);
                    if (result != null)
                    {
                        path.Add(paragraph);
                        return result;
                    }
                        
                }
            }
            return null;
        }

        public PreambleParagraph CreatePreambleParagraph()
        {
            var paragraph = new PreambleParagraph();
            this.Preamble.Paragraphs.Add(paragraph);
            return paragraph;
        }

        public OperativeParagraph CreateOperativeParagraph(string text = "")
        {
            var paragraph = new OperativeParagraph();
            paragraph.Text = text;
            this.OperativeSection.Paragraphs.Add(paragraph);
            return paragraph;
        }

        public OperativeParagraph CreateChildParagraph(string parentId, string text = "")
        {
            var parentParagraph = FindOperativeParagraph(parentId);
            if (parentParagraph == null)
                throw new Exceptions.Resolution.OperativeParagraphNotFoundException();

            var newParagraph = new OperativeParagraph();
            newParagraph.Text = text;
            parentParagraph.Children.Add(newParagraph);
            return newParagraph;
        }

        public OperativeParagraph CreateChildParagraph(OperativeParagraph parent, string text = "") => CreateChildParagraph(parent.OperativeParagraphId, text);

        public List<OperativeParagraph> GetOperativeParagraphPath(string id)
        {
            var path = new List<OperativeParagraph>();
            foreach (var paragraph in OperativeSection.Paragraphs)
            {
                var result = FindOperativeParagraphPathRecursive(paragraph, id, path);
                if (result != null)
                {
                    path.Reverse();
                    return path;
                }
            }
            return null;
        }

        public void RemoveOperativeParagraph(OperativeParagraph paragraph)
        {
            var path = GetOperativeParagraphPath(paragraph.OperativeParagraphId);
            if (!path.Any())
                throw new Exceptions.Resolution.OperativeParagraphNotFoundException();
            if (path.Count == 1)
            {
                this.OperativeSection.Paragraphs.Remove(paragraph);
            }
            else
            {
                path[path.Count - 1].Children.Remove(paragraph);
            }

            foreach(var amendment in this.OperativeSection.AddAmendments.Where(n => n.TargetSectionId == paragraph.OperativeParagraphId))
            {
                RemoveAmendment(amendment);
            }
        }

        public List<IAmendment> AmendmentsForOperativeParagraph(string id)
        {
            var result = new List<IAmendment>();

            result.AddRange(this.OperativeSection.AddAmendments.Where(n => n.TargetSectionId == id));
            result.AddRange(this.OperativeSection.ChangeAmendments.Where(n => n.TargetSectionId == id));
            result.AddRange(this.OperativeSection.DeleteAmendments.Where(n => n.TargetSectionId == id));
            result.AddRange(this.OperativeSection.MoveAmendments.Where(n => n.TargetSectionId == id));
            return result;
        }

        private void PushAmendment(IAmendment amendment)
        {
            if (this.FindOperativeParagraph(amendment.TargetSectionId) == null)
                throw new Exceptions.Resolution.OperativeParagraphNotFoundException();

            if (amendment is AddAmendment addAmendment)
            {
                this.OperativeSection.AddAmendments.Add(addAmendment);
            }
            else if (amendment is ChangeAmendment changeAmendment)
            {
                this.OperativeSection.ChangeAmendments.Add(changeAmendment);
            }
            else if (amendment is DeleteAmendment deleteAmendment)
            {
                this.OperativeSection.DeleteAmendments.Add(deleteAmendment);
            }
            else if (amendment is MoveAmendment moveAmendment)
            {
                this.OperativeSection.MoveAmendments.Add(moveAmendment);
            }
            else
            {
                throw new Exceptions.Resolution.UnsupportedAmendmentTypeException();
            }
        }

        public void RemoveAmendment(IAmendment amendment)
        {
            if (amendment is AddAmendment addAmendment)
            {
                this.OperativeSection.AddAmendments.Remove(addAmendment);
            }
            else if (amendment is ChangeAmendment changeAmendment)
            {
                this.OperativeSection.ChangeAmendments.Remove(changeAmendment);
            }
            else if (amendment is DeleteAmendment deleteAmendment)
            {
                this.OperativeSection.DeleteAmendments.Remove(deleteAmendment);
            }
            else if (amendment is MoveAmendment moveAmendment)
            {
                this.OperativeSection.MoveAmendments.Remove(moveAmendment);
                this.OperativeSection.Paragraphs.RemoveAll(n => moveAmendment.NewTargetSectionId == n.OperativeParagraphId);
            }
            else
            {
                throw new Exceptions.Resolution.UnsupportedAmendmentTypeException();
            }
        }

        public DeleteAmendment CreateDeleteAmendment(string paragraphId)
        {
            if (this.FindOperativeParagraph(paragraphId) == null)
                throw new Exceptions.Resolution.OperativeParagraphNotFoundException();

            DeleteAmendment newAmendment = new DeleteAmendment();
            newAmendment.TargetSectionId = paragraphId;
            PushAmendment(newAmendment);
            return newAmendment;
        }

        public DeleteAmendment CreateDeleteAmendment(OperativeParagraph paragraph) => CreateDeleteAmendment(paragraph.OperativeParagraphId);

        public ChangeAmendment CreateChangeAmendment(string paragraphId, string newText = "")
        {
            if (this.FindOperativeParagraph(paragraphId) == null)
                throw new Exceptions.Resolution.OperativeParagraphNotFoundException();

            var newAmendment = new ChangeAmendment();
            newAmendment.TargetSectionId = paragraphId;
            newAmendment.NewText = newText;
            PushAmendment(newAmendment);
            return newAmendment;
        }

        public ChangeAmendment CreateChangeAmendment(OperativeParagraph paragraph, string newText = "") => CreateChangeAmendment(paragraph.OperativeParagraphId, newText);

        public MoveAmendment CreateMoveAmendment(string paragraphId, int targetIndex, OperativeParagraph parentParagraph = null)
        {
            if (this.FindOperativeParagraph(paragraphId) == null)
                throw new Exceptions.Resolution.OperativeParagraphNotFoundException();

            

            var newAmendment = new MoveAmendment();
            newAmendment.TargetSectionId = paragraphId;
            var virtualParagraph = new OperativeParagraph();
            virtualParagraph.IsLocked = true;
            virtualParagraph.IsVirtual = true;
            newAmendment.NewTargetSectionId = virtualParagraph.OperativeParagraphId;
            InsertIntoRealPosition(virtualParagraph, targetIndex, parentParagraph);
            PushAmendment(newAmendment);
            return newAmendment;
        }

        public MoveAmendment CreateMoveAmendment(OperativeParagraph paragraph, int targetIndex, OperativeParagraph parentParagraph = null) =>
            CreateMoveAmendment(paragraph.OperativeParagraphId, targetIndex, parentParagraph);
            



        public AddAmendment CreateAddAmendment(int targetIndex, string text = "", OperativeParagraph parentParagraph = null)
        {
            var virtualParagraph = new OperativeParagraph(text);
            virtualParagraph.IsVirtual = true;
            virtualParagraph.Visible = false;
            var position = InsertIntoRealPosition(virtualParagraph, targetIndex, parentParagraph);
            var amendment = new AddAmendment();
            amendment.Position = position;
            amendment.TargetSectionId = virtualParagraph.OperativeParagraphId;
            PushAmendment(amendment);
            return amendment;
        }

        private int InsertIntoRealPosition(OperativeParagraph paragraph, int targetIndex, OperativeParagraph parentParagraph)
        {
            if (parentParagraph == null)
            {
                if (targetIndex != 0)
                {
                    var realParagraphs = OperativeSection.Paragraphs.Where(n => n.IsVirtual == false);
                    if (realParagraphs.Count() < targetIndex)
                    {
                        var paragraphAtThisIndex = realParagraphs.ElementAt(targetIndex);
                        var realIndexOfParagraph = OperativeSection.Paragraphs.IndexOf(paragraphAtThisIndex);
                        targetIndex = realIndexOfParagraph + 1;
                    }
                    else
                    {
                        targetIndex = realParagraphs.Count();
                    }
                    if (targetIndex > this.OperativeSection.Paragraphs.Count) targetIndex = this.OperativeSection.Paragraphs.Count;
                }
                this.OperativeSection.Paragraphs.Insert(targetIndex, paragraph);
            }
            else
            {
                if (this.FindOperativeParagraph(parentParagraph.OperativeParagraphId) == null)
                    throw new Exceptions.Resolution.OperativeParagraphNotFoundException("Target parent Paragraph not found in this Resolution");

                parentParagraph.Children.Insert(targetIndex, paragraph);
            }
            return targetIndex;
        }

        public Resolution()
        {
            ResolutionId = Guid.NewGuid().ToString();
            Preamble = new ResolutionPreamble();
            OperativeSection = new OperativeSection();
            Header = new ResolutionHeader();
        }
    }
}
