using MUNityClient.Models.Resolution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MUNityClient.Extensions.ResolutionExtensions
{
    public static class OperativeParagraphTools
    {
        public static OperativeParagraph CreateOperativeParagraph(this Resolution resolution, string text = "")
        {
            var paragraph = new OperativeParagraph();
            paragraph.Text = text;
            resolution.OperativeSection.Paragraphs.Add(paragraph);
            return paragraph;
        }

        public static OperativeParagraph CreateChildParagraph(this Resolution resolution, string parentId, string text = "")
        {
            var parentParagraph = resolution.FindOperativeParagraph(parentId);
            if (parentParagraph == null)
                throw new Exceptions.Resolution.OperativeParagraphNotFoundException();

            var newParagraph = new OperativeParagraph();
            newParagraph.Text = text;
            parentParagraph.Children.Add(newParagraph);
            return newParagraph;
        }

        public static OperativeParagraph CreateChildParagraph(this Resolution resolution, OperativeParagraph parent, string text = "") 
            => resolution.CreateChildParagraph(parent.OperativeParagraphId, text);

        public static OperativeParagraph FindOperativeParagraph(this Resolution resolution, string id)
        {
            foreach (var paragraph in resolution.OperativeSection.Paragraphs)
            {
                var result = FindOperativeParagraphRecursive(paragraph, id);
                if (result != null) return result;
            }
            return null;
        }

        private static OperativeParagraph FindOperativeParagraphRecursive(OperativeParagraph paragraph, string targetId)
        {
            if (paragraph.OperativeParagraphId == targetId) return paragraph;
            if (paragraph.Children != null && paragraph.Children.Any())
            {
                foreach (var child in paragraph.Children)
                {
                    var result = FindOperativeParagraphRecursive(child, targetId);
                    if (result != null) return result;
                }
            }
            return null;
        }

        private static OperativeParagraph FindOperativeParagraphPathRecursive(OperativeParagraph paragraph, string targetId, List<OperativeParagraph> path)
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

        public static List<OperativeParagraph> GetOperativeParagraphPath(this Resolution resolution, string id)
        {
            var path = new List<OperativeParagraph>();
            foreach (var paragraph in resolution.OperativeSection.Paragraphs)
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

        public static void RemoveOperativeParagraph(this Resolution resolution, OperativeParagraph paragraph)
        {
            var path = resolution.GetOperativeParagraphPath(paragraph.OperativeParagraphId);
            if (!path.Any())
                throw new Exceptions.Resolution.OperativeParagraphNotFoundException();
            if (path.Count == 1)
            {
                resolution.OperativeSection.Paragraphs.Remove(paragraph);
            }
            else
            {
                path[path.Count - 1].Children.Remove(paragraph);
            }

            // TODO: Remove all Amendments of this paragraph and all its child paragraphs!
            foreach (var amendment in resolution.OperativeSection.AddAmendments.Where(n => n.TargetSectionId == paragraph.OperativeParagraphId))
            {
                resolution.RemoveAmendment(amendment);
            }
        }


        public static List<(string id, string path, string text)> GetRealOperativeParagraphsInfo(this Resolution resolution)
        {
            var list = new List<(string id, string path, string text)>();
            var realParagraphs = resolution.OperativeSection.Paragraphs.Where(n => !n.IsVirtual);
            int index = 1;
            foreach (var paragraph in realParagraphs)
            {
                string prePath = index.ToString();
                AddRealOperativeParagraphInfoRec(prePath, paragraph, list);
                index++;
            }
            return list;
        }

        private static void AddRealOperativeParagraphInfoRec(string prePath, OperativeParagraph paragraph, List<(string id, string path, string text)> list)
        {
            var newElement = (id: paragraph.OperativeParagraphId, path: prePath, text: paragraph.Text);
            list.Add(newElement);
            if (paragraph.Children != null && paragraph.Children.Any())
            {
                var realParagraphs = paragraph.Children.Where(n => !n.IsVirtual);
                int index = 1;
                prePath += ".";
                int level = prePath.Count(n => n == '.');
                foreach (var childParagraph in realParagraphs)
                {
                    var newPath = prePath;
                    if (level == 1 || level % 4 == 1) newPath += Conversion.ToLetter(index - 1);
                    else if (level == 2 || level % 5 == 2) newPath += Conversion.ToRoman(index).ToLower();
                    AddRealOperativeParagraphInfoRec(newPath, childParagraph, list);
                    index++;
                }
            }
        }

        public static int InsertIntoRealPosition(this Resolution resolution, OperativeParagraph paragraph, int targetIndex, OperativeParagraph parentParagraph)
        {
            if (parentParagraph == null)
            {
                if (targetIndex > resolution.OperativeSection.Paragraphs.Count) targetIndex = resolution.OperativeSection.Paragraphs.Count;
                resolution.OperativeSection.Paragraphs.Insert(targetIndex, paragraph);
            }
            else
            {
                if (resolution.FindOperativeParagraph(parentParagraph.OperativeParagraphId) == null)
                    throw new Exceptions.Resolution.OperativeParagraphNotFoundException("Target parent Paragraph not found in this Resolution");

                parentParagraph.Children.Insert(targetIndex, paragraph);
            }
            return targetIndex;
        }

        /// <summary>
        /// Returns the displayed Index name of a Paragraph for example 
        /// 1, 2, 2.a, 2.a.i etc.
        /// </summary>
        /// <param name="paragraph"></param>
        /// <returns></returns>
        public static string GetIndexNameOfOperativeParagraph(this Resolution resolution, string paragraphId)
        {
            var path = resolution.GetOperativeParagraphPath(paragraphId);
            var numbers = new List<int>();
            OperativeParagraph parent = null;
            foreach (var paragraph in path)
            {
                if (parent == null)
                {
                    numbers.Add(resolution.OperativeSection.Paragraphs.Where(n => !n.IsVirtual).ToList().IndexOf(paragraph));
                }
                else
                {
                    numbers.Add(parent.Children.Where(n => !n.IsVirtual).ToList().IndexOf(paragraph));
                }
                parent = paragraph;
            }
            return Conversion.ToPathname(numbers.ToArray());
        }

        public static int IndexOfParagraph(this Resolution resolution, OperativeParagraph paragraph)
        {
            int index = resolution.OperativeSection.Paragraphs.IndexOf(paragraph);
            if (index != -1) return index;
            var path = resolution.GetOperativeParagraphPath(paragraph.OperativeParagraphId);
            var parentElement = path[path.Count - 1];
            return parentElement.Children.IndexOf(paragraph);
        }

        

        public static List<string> GetAllOperativeParagraphIds(this Resolution resolution)
        {
            var list = new List<string>();
            list.AddRange(resolution.OperativeSection.Paragraphs.Select(n => n.OperativeParagraphId));
            resolution.OperativeSection.Paragraphs.ForEach(n => AddAllChildrenRecursive(n, list));
            return list;
        }

        #region function linking
            public static string GetIndexNameOfOperativeParagraph(this Resolution resolution, OperativeParagraph paragraph) => resolution.GetIndexNameOfOperativeParagraph(paragraph.OperativeParagraphId);
        #endregion

        #region internal
        private static void AddAllChildrenRecursive(OperativeParagraph paragraph, List<string> list)
        {
            if (paragraph.Children != null && paragraph.Children.Any())
            {
                list.AddRange(paragraph.Children.Select(n => n.OperativeParagraphId));
                paragraph.Children.ForEach(n => AddAllChildrenRecursive(n, list));
            }
        }
        #endregion
    }
}
