using MUNityClient.Models.Resolution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MUNityClient.Extensions.ResolutionExtensions
{
    public static class AmendmentTools
    {

        /// <summary>
        /// Returns all the Amendments for the operative paragraph with the given Id.
        /// </summary>
        /// <param name="section"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static List<IAmendment> AmendmentsForOperativeParagraph(this OperativeSection section, string id)
        {
            var result = new List<IAmendment>();

            result.AddRange(section.AddAmendments.Where(n => n.TargetSectionId == id));
            result.AddRange(section.ChangeAmendments.Where(n => n.TargetSectionId == id));
            result.AddRange(section.DeleteAmendments.Where(n => n.TargetSectionId == id));
            result.AddRange(section.MoveAmendments.Where(n => n.TargetSectionId == id));
            return result;
        }

        /// <summary>
        /// Adds a new Amendment into the Amendment list.
        /// </summary>
        /// <param name="section"></param>
        /// <param name="amendment"></param>
        private static void PushAmendment(this OperativeSection section, IAmendment amendment)
        {
            // For now every Amendment has a TargetSectionId this could maybe be different one day
            // Remember to move this function if this day ever comes.
            if (section.FirstOrDefault(n => n.OperativeParagraphId == amendment.TargetSectionId) == null)
                throw new Exceptions.Resolution.OperativeParagraphNotFoundException();

            if (amendment is AddAmendment addAmendment)
            {
                section.AddAmendments.Add(addAmendment);
            }
            else if (amendment is ChangeAmendment changeAmendment)
            {
                section.ChangeAmendments.Add(changeAmendment);
            }
            else if (amendment is DeleteAmendment deleteAmendment)
            {
                section.DeleteAmendments.Add(deleteAmendment);
            }
            else if (amendment is MoveAmendment moveAmendment)
            {
                section.MoveAmendments.Add(moveAmendment);
            }
            else
            {
                throw new Exceptions.Resolution.UnsupportedAmendmentTypeException();
            }
        }

        /// <summary>
        /// Removes an amentment from its list. Note that this is not the same
        /// as Deny. This will just remove the given instance of the amendment.
        /// </summary>
        /// <param name="section"></param>
        /// <param name="amendment"></param>
        public static void RemoveAmendment(this OperativeSection section, IAmendment amendment)
        {
            if (amendment is AddAmendment addAmendment)
            {
                section.AddAmendments.RemoveAll(n => n.TargetSectionId == amendment.TargetSectionId);
            }
            else if (amendment is ChangeAmendment changeAmendment)
            {
                section.ChangeAmendments.Remove(changeAmendment);
            }
            else if (amendment is DeleteAmendment deleteAmendment)
            {
                section.DeleteAmendments.Remove(deleteAmendment);
            }
            else if (amendment is MoveAmendment moveAmendment)
            {
                section.MoveAmendments.Remove(moveAmendment);
                section.Paragraphs.RemoveAll(n => moveAmendment.NewTargetSectionId == n.OperativeParagraphId);
            }
            else
            {
                throw new Exceptions.Resolution.UnsupportedAmendmentTypeException();
            }
        }

        /// <summary>
        /// Creates a new Amendment to delete an operative paragraph.
        /// </summary>
        /// <param name="section"></param>
        /// <param name="paragraphId"></param>
        /// <returns></returns>
        public static DeleteAmendment CreateDeleteAmendment(this OperativeSection section, string paragraphId)
        {
            if (section.FindOperativeParagraph(paragraphId) == null)
                throw new Exceptions.Resolution.OperativeParagraphNotFoundException();

            DeleteAmendment newAmendment = new DeleteAmendment();
            newAmendment.TargetSectionId = paragraphId;
            section.PushAmendment(newAmendment);
            return newAmendment;
        }

        public static ChangeAmendment CreateChangeAmendment(this OperativeSection section, string paragraphId, string newText = "")
        {
            if (section.FindOperativeParagraph(paragraphId) == null)
                throw new Exceptions.Resolution.OperativeParagraphNotFoundException();

            var newAmendment = new ChangeAmendment();
            newAmendment.TargetSectionId = paragraphId;
            newAmendment.NewText = newText;
            section.PushAmendment(newAmendment);
            return newAmendment;
        }

        

        public static MoveAmendment CreateMoveAmendment(this OperativeSection section, string paragraphId, int targetIndex, OperativeParagraph parentParagraph = null)
        {
            var sourceParagraph = section.FindOperativeParagraph(paragraphId);
            if (sourceParagraph == null)
                throw new Exceptions.Resolution.OperativeParagraphNotFoundException();

            var newAmendment = new MoveAmendment();
            newAmendment.TargetSectionId = paragraphId;
            var virtualParagraph = new OperativeParagraph();
            virtualParagraph.IsLocked = true;
            virtualParagraph.IsVirtual = true;
            virtualParagraph.Text = sourceParagraph.Text;
            newAmendment.NewTargetSectionId = virtualParagraph.OperativeParagraphId;
            section.InsertIntoRealPosition(virtualParagraph, targetIndex, parentParagraph);
            section.PushAmendment(newAmendment);
            return newAmendment;
        }


        public static AddAmendment CreateAddAmendment(this OperativeSection section, int targetIndex, string text = "", OperativeParagraph parentParagraph = null)
        {
            var virtualParagraph = new OperativeParagraph(text);
            virtualParagraph.IsVirtual = true;
            virtualParagraph.Visible = false;
            var position = section.InsertIntoRealPosition(virtualParagraph, targetIndex, parentParagraph);
            var amendment = new AddAmendment();
            amendment.TargetSectionId = virtualParagraph.OperativeParagraphId;
            section.PushAmendment(amendment);
            return amendment;
        }

        public static DeleteAmendment CreateDeleteAmendment(this OperativeSection section, OperativeParagraph paragraph) => section.CreateDeleteAmendment(paragraph.OperativeParagraphId);

        /// <summary>
        /// Creates a new Move Amendment. The given targetIndex is the position the new Virtual Paragraph will be located.
        /// Type in 0 to move it to the beginning of the list. Note that 1 will also move it to the start!
        /// When you have two Paragraphs (A and B) and want A to move behind B (B, A) you need to set the targetIndex to 2!
        /// </summary>
        /// <param name="paragraph"></param>
        /// <param name="targetIndex"></param>
        /// <param name="parentParagraph"></param>
        /// <returns></returns>
        public static MoveAmendment CreateMoveAmendment(this OperativeSection section, OperativeParagraph paragraph, int targetIndex, OperativeParagraph parentParagraph = null) =>
            section.CreateMoveAmendment(paragraph.OperativeParagraphId, targetIndex, parentParagraph);

        public static ChangeAmendment CreateChangeAmendment(this OperativeSection section, OperativeParagraph paragraph, string newText = "") => section.CreateChangeAmendment(paragraph.OperativeParagraphId, newText);
    }
}
