using MUNityClient.Models.Resolution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MUNityClient.Extensions.ResolutionExtensions
{
    public static class OperativeParagraphTools
    {
        private static IEnumerable<string> OperativeParagraphOperators
        {
            get
            {
                yield return "akzeptiert";
                yield return "anerkennt";
                yield return "appelliert eindringlich";
                yield return "appelliert";
                yield return "beauftrag";
                yield return "bedauert";
                yield return "bedenkt";
                yield return "befürwortet";
                yield return "begrüßt (wärmstens)";
                yield return "behält sich vor";
                yield return "beklagt";
                yield return "bekräftigt";
                yield return "bekundet.*hocherfreut";
                yield return "bekundet";
                yield return "bemerkt";
                yield return "bestätigt";
                yield return "betont";
                yield return "betrachtet";
                yield return "billigt";
                yield return "bittet.*nachdrücklich";
                yield return "bittet";
                yield return "dankt";
                yield return "delegiert";
                yield return "drängt";
                yield return "empfiehlt dringend";
                yield return "empfiehlt";
                yield return "entschließt sich";
                yield return "erinnert an";
                yield return "erinnert";
                yield return "erkennt an";
                yield return "erklärt erneut";
                yield return "erklärt";
                yield return "ermutigt";
                yield return "ernennt";
                yield return "ersucht";
                yield return "erwägt";
                yield return "fordert.*auf";
                yield return "gratuliert";
                yield return "hebt hervor";
                yield return "hofft";
                yield return "ist sich bewusst";
                yield return "ist fest überzeugt";
                yield return "ist überzeugt";
                yield return "kommt überein";
                yield return "kommt zu dem Schluss";
                yield return "kommt zu der Überzeugung";
                yield return "legt nahe";
                yield return "legt dringend nahe";
                yield return "lobt feierlich";
                yield return "macht Gebraucht von";
                yield return "macht sich zu eigen";
                yield return "nimmt an";
                yield return "nimmt hocherfreut zur Kenntnis";
                yield return "nimmt mit Bedauern zur Kenntnis";
                yield return "nimmt zur Kenntnis";
                yield return "räumt ein";
                yield return "ruft abermals auf";
                yield return "ruft auf";
                yield return "schlägt vor";
                yield return "schließt sich an";
                yield return "setzt von neuem ein";
                yield return "setzt ein";
                yield return "stellt fest";
                yield return "unterstreicht";
                yield return "unterstützt";
                yield return "verlangt unmissverständlich";
                yield return "vermerkt";
                yield return "verpflichtet sich";
                yield return "versichert";
                yield return "verurteilt.*entschieden";
                yield return "verurteilt";
                yield return "verweist";
                yield return "wiederholt";
                yield return "weist auf die Tatsache hin";
                yield return "weist auf.*hin";
                yield return "würdigt";
                yield return "zieht ernsthaft in Erwägung";
                yield return "zieht in Erwägung";

                // Sicherheitsrat Operatoren
                yield return "autorisiert";
                yield return "beschließt";
                yield return "entsendet";
                yield return "verabschiedet";
                yield return "verschärft";
            }
        }

        public static OperativeParagraph CreateOperativeParagraph(this OperativeSection section, string text = "")
        {
            var paragraph = new OperativeParagraph();
            paragraph.Text = text;
            section.Paragraphs.Add(paragraph);
            return paragraph;
        }

        public static OperativeParagraph CreateChildParagraph(this OperativeSection section, string parentId, string text = "")
        {
            var parentParagraph = section.FindOperativeParagraph(parentId);
            if (parentParagraph == null)
                throw new Exceptions.Resolution.OperativeParagraphNotFoundException();

            var newParagraph = new OperativeParagraph();
            newParagraph.Text = text;
            parentParagraph.Children.Add(newParagraph);
            return newParagraph;
        }

        public static OperativeParagraph CreateChildParagraph(this OperativeSection section, OperativeParagraph parent, string text = "") 
            => section.CreateChildParagraph(parent.OperativeParagraphId, text);

        public static OperativeParagraph FindOperativeParagraph(this OperativeSection section, string id)
        {
            return section.FirstOrDefault(n => n.OperativeParagraphId == id);
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

        public static List<OperativeParagraph> GetOperativeParagraphPath(this OperativeSection section, string id)
        {
            var path = new List<OperativeParagraph>();
            foreach (var paragraph in section.Paragraphs)
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

        public static void RemoveOperativeParagraph(this OperativeSection section, OperativeParagraph paragraph)
        {
            var path = section.GetOperativeParagraphPath(paragraph.OperativeParagraphId);
            if (!path.Any())
                throw new Exceptions.Resolution.OperativeParagraphNotFoundException();
            if (path.Count == 1)
            {
                section.Paragraphs.Remove(paragraph);
            }
            else
            {
                path[path.Count - 1].Children.Remove(paragraph);
            }

            // TODO: Remove all Amendments of this paragraph and all its child paragraphs!
            foreach (var amendment in section.AddAmendments.Where(n => n.TargetSectionId == paragraph.OperativeParagraphId))
            {
                section.RemoveAmendment(amendment);
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

        public static int InsertIntoRealPosition(this OperativeSection section, OperativeParagraph paragraph, int targetIndex, OperativeParagraph parentParagraph)
        {
            if (parentParagraph == null)
            {
                if (targetIndex > section.Paragraphs.Count) targetIndex = section.Paragraphs.Count;
                section.Paragraphs.Insert(targetIndex, paragraph);
            }
            else
            {
                if (section.FindOperativeParagraph(parentParagraph.OperativeParagraphId) == null)
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
        public static string GetIndexNameOfOperativeParagraph(this OperativeSection section, string paragraphId)
        {
            var path = section.GetOperativeParagraphPath(paragraphId);
            var numbers = new List<int>();
            OperativeParagraph parent = null;
            foreach (var paragraph in path)
            {
                if (parent == null)
                {
                    numbers.Add(section.Paragraphs.Where(n => !n.IsVirtual).ToList().IndexOf(paragraph));
                }
                else
                {
                    numbers.Add(parent.Children.Where(n => !n.IsVirtual).ToList().IndexOf(paragraph));
                }
                parent = paragraph;
            }
            return Conversion.ToPathname(numbers.ToArray());
        }

        public static int IndexOfParagraph(this OperativeSection section, OperativeParagraph paragraph)
        {
            int index = section.Paragraphs.IndexOf(paragraph);
            if (index != -1) return index;
            var path = section.GetOperativeParagraphPath(paragraph.OperativeParagraphId);
            var parentElement = path[path.Count - 1];
            return parentElement.Children.IndexOf(paragraph);
        }

        

        public static List<string> GetAllOperativeParagraphIds(this OperativeSection section)
        {
            var list = new List<string>();
            list.AddRange(section.Paragraphs.Select(n => n.OperativeParagraphId));
            section.Paragraphs.ForEach(n => AddAllChildrenRecursive(n, list));
            return list;
        }

        public static List<OperativeParagraph> WhereParagraph(this OperativeSection operativeSection, Func<OperativeParagraph, bool> predicate)
        {
            var list = new List<OperativeParagraph>();
            list.AddRange(operativeSection.Paragraphs.Where(predicate));
            operativeSection.Paragraphs.ForEach(n => deepWhere(n, predicate, list));
            return list;
        }

        private static void deepWhere(OperativeParagraph parentParagraph, Func<OperativeParagraph, bool> predicate, List<OperativeParagraph> resultList)
        {
            if (parentParagraph.Children != null && parentParagraph.Children.Any())
            {
                resultList.AddRange(parentParagraph.Children.Where(predicate));
                parentParagraph.Children.ForEach(n => deepWhere(n, predicate, resultList));
            }
        }

        public static OperativeParagraph FirstOrDefault(this OperativeSection operativeSection, Func<OperativeParagraph, bool> predicate)
        {
            var result = operativeSection.Paragraphs.FirstOrDefault(predicate);
            if (result != null) return result;
            foreach(var s in operativeSection.Paragraphs)
            {
                result = deepFirstOrDefault(s, predicate);
                if (result != null) return result;
            }
            return null;
        }

        private static OperativeParagraph deepFirstOrDefault(this OperativeParagraph paragraph, Func<OperativeParagraph, bool> predicate)
        {
            var result = paragraph.Children.FirstOrDefault(predicate);
            if (result != null) return result;
            foreach(var child in paragraph.Children)
            {
                return deepFirstOrDefault(child, predicate);
            }
            return null;
        }

        public static bool HasValidOperator(this OperativeParagraph paragraph)
        {
            foreach(var op in OperativeParagraphOperators)
            {
                if (op.Contains(".*"))
                {
                    // Regex Match
                    if (System.Text.RegularExpressions.Regex.Match(paragraph.Text, op).Success)
                        return true;
                }
                else
                {
                    if (paragraph.Text.StartsWith("_" + op + "_") || paragraph.Text.StartsWith(op + " ") || paragraph.Text == op)
                        return true;
                }
            }

            return false;
        }

        #region function linking
            public static string GetIndexNameOfOperativeParagraph(this OperativeSection section, OperativeParagraph paragraph) => section.GetIndexNameOfOperativeParagraph(paragraph.OperativeParagraphId);
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
