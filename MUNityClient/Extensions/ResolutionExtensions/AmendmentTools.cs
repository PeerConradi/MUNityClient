using MUNityClient.Models.Resolution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MUNityClient.Extensions.ResolutionExtensions
{
    public static class AmendmentTools
    {
        public static List<IAmendment> AmendmentsForOperativeParagraph(this Resolution resolution, string id)
        {
            var result = new List<IAmendment>();

            result.AddRange(resolution.OperativeSection.AddAmendments.Where(n => n.TargetSectionId == id));
            result.AddRange(resolution.OperativeSection.ChangeAmendments.Where(n => n.TargetSectionId == id));
            result.AddRange(resolution.OperativeSection.DeleteAmendments.Where(n => n.TargetSectionId == id));
            result.AddRange(resolution.OperativeSection.MoveAmendments.Where(n => n.TargetSectionId == id));
            return result;
        }

        private static void PushAmendment(this Resolution resolution, IAmendment amendment)
        {
            // For now every Amendment has a TargetSectionId this could maybe be different one day
            // Remember to move this function if this day ever comes.
            if (resolution.OperativeSection.FirstOrDefault(n => n.OperativeParagraphId == amendment.TargetSectionId) == null)
                throw new Exceptions.Resolution.OperativeParagraphNotFoundException();

            if (amendment is AddAmendment addAmendment)
            {
                resolution.OperativeSection.AddAmendments.Add(addAmendment);
            }
            else if (amendment is ChangeAmendment changeAmendment)
            {
                resolution.OperativeSection.ChangeAmendments.Add(changeAmendment);
            }
            else if (amendment is DeleteAmendment deleteAmendment)
            {
                resolution.OperativeSection.DeleteAmendments.Add(deleteAmendment);
            }
            else if (amendment is MoveAmendment moveAmendment)
            {
                resolution.OperativeSection.MoveAmendments.Add(moveAmendment);
            }
            else
            {
                throw new Exceptions.Resolution.UnsupportedAmendmentTypeException();
            }
        }

        public static void RemoveAmendment(this Resolution resolution, IAmendment amendment)
        {
            if (amendment is AddAmendment addAmendment)
            {
                resolution.OperativeSection.AddAmendments.RemoveAll(n => n.TargetSectionId == amendment.TargetSectionId);
            }
            else if (amendment is ChangeAmendment changeAmendment)
            {
                resolution.OperativeSection.ChangeAmendments.Remove(changeAmendment);
            }
            else if (amendment is DeleteAmendment deleteAmendment)
            {
                resolution.OperativeSection.DeleteAmendments.Remove(deleteAmendment);
            }
            else if (amendment is MoveAmendment moveAmendment)
            {
                resolution.OperativeSection.MoveAmendments.Remove(moveAmendment);
                resolution.OperativeSection.Paragraphs.RemoveAll(n => moveAmendment.NewTargetSectionId == n.OperativeParagraphId);
            }
            else
            {
                throw new Exceptions.Resolution.UnsupportedAmendmentTypeException();
            }
        }

        public static DeleteAmendment CreateDeleteAmendment(this Resolution resolution, string paragraphId)
        {
            if (resolution.FindOperativeParagraph(paragraphId) == null)
                throw new Exceptions.Resolution.OperativeParagraphNotFoundException();

            DeleteAmendment newAmendment = new DeleteAmendment();
            newAmendment.TargetSectionId = paragraphId;
            resolution.PushAmendment(newAmendment);
            return newAmendment;
        }

        public static DeleteAmendment CreateDeleteAmendment(this Resolution resolution, OperativeParagraph paragraph) => resolution.CreateDeleteAmendment(paragraph.OperativeParagraphId);

        public static ChangeAmendment CreateChangeAmendment(this Resolution resolution, string paragraphId, string newText = "")
        {
            if (resolution.FindOperativeParagraph(paragraphId) == null)
                throw new Exceptions.Resolution.OperativeParagraphNotFoundException();

            var newAmendment = new ChangeAmendment();
            newAmendment.TargetSectionId = paragraphId;
            newAmendment.NewText = newText;
            resolution.PushAmendment(newAmendment);
            return newAmendment;
        }

        public static ChangeAmendment CreateChangeAmendment(this Resolution resolution, OperativeParagraph paragraph, string newText = "") => resolution.CreateChangeAmendment(paragraph.OperativeParagraphId, newText);

        public static MoveAmendment CreateMoveAmendment(this Resolution resolution, string paragraphId, int targetIndex, OperativeParagraph parentParagraph = null)
        {
            var sourceParagraph = resolution.FindOperativeParagraph(paragraphId);
            if (sourceParagraph == null)
                throw new Exceptions.Resolution.OperativeParagraphNotFoundException();

            var newAmendment = new MoveAmendment();
            newAmendment.TargetSectionId = paragraphId;
            var virtualParagraph = new OperativeParagraph();
            virtualParagraph.IsLocked = true;
            virtualParagraph.IsVirtual = true;
            virtualParagraph.Text = sourceParagraph.Text;
            newAmendment.NewTargetSectionId = virtualParagraph.OperativeParagraphId;
            resolution.InsertIntoRealPosition(virtualParagraph, targetIndex, parentParagraph);
            resolution.PushAmendment(newAmendment);
            return newAmendment;
        }

        /// <summary>
        /// Creates a new Move Amendment. The given targetIndex is the position the new Virtual Paragraph will be located.
        /// Type in 0 to move it to the beginning of the list. Note that 1 will also move it to the start!
        /// When you have two Paragraphs (A and B) and want A to move behind B (B, A) you need to set the targetIndex to 2!
        /// </summary>
        /// <param name="paragraph"></param>
        /// <param name="targetIndex"></param>
        /// <param name="parentParagraph"></param>
        /// <returns></returns>
        public static MoveAmendment CreateMoveAmendment(this Resolution resolution, OperativeParagraph paragraph, int targetIndex, OperativeParagraph parentParagraph = null) =>
            resolution.CreateMoveAmendment(paragraph.OperativeParagraphId, targetIndex, parentParagraph);




        public static AddAmendment CreateAddAmendment(this Resolution resolution, int targetIndex, string text = "", OperativeParagraph parentParagraph = null)
        {
            var virtualParagraph = new OperativeParagraph(text);
            virtualParagraph.IsVirtual = true;
            virtualParagraph.Visible = false;
            var position = resolution.InsertIntoRealPosition(virtualParagraph, targetIndex, parentParagraph);
            var amendment = new AddAmendment();
            amendment.TargetSectionId = virtualParagraph.OperativeParagraphId;
            resolution.PushAmendment(amendment);
            return amendment;
        }
    }
}
