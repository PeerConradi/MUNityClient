using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using MUNityClient.Models.Resolution;

namespace MUNityClientTest.ResolutionTest
{
    public class TestResolutionGeneral
    {
        [Test]
        public void TestCreateResolutionInstance()
        {
            var instance = new Resolution();
            Assert.NotNull(instance);
            Assert.NotNull(instance.Header);
            Assert.NotNull(instance.Preamble);
            Assert.NotNull(instance.Preamble.Paragraphs);
            Assert.NotNull(instance.OperativeSection);
            Assert.NotNull(instance.OperativeSection.Paragraphs, "Expecting a new created Resolution to have at least an empty list in paragraphs");
            Assert.NotNull(instance.OperativeSection.AddAmendments);
            Assert.NotNull(instance.OperativeSection.ChangeAmendments);
            Assert.NotNull(instance.OperativeSection.DeleteAmendments);
            Assert.NotNull(instance.OperativeSection.MoveAmendments);
        }

        [Test]
        public void TestCanCreatePreambleParagraph()
        {
            var instance = new Resolution();
            var paragraph = instance.CreatePreambleParagraph();
            Assert.NotNull(paragraph);
            Assert.Contains(paragraph, instance.Preamble.Paragraphs);
        }

        [Test]
        public void TestCanCreateOperativeParagraph()
        {
            var instance = new Resolution();
            var paragraph = instance.CreateOperativeParagraph();
            Assert.NotNull(paragraph);
            Assert.Contains(paragraph, instance.OperativeSection.Paragraphs);
        }

        [Test]
        public void FindTopLevelOperativeParagraph()
        {
            var resoltution = new Resolution();
            var paragraphOne = resoltution.CreateOperativeParagraph();
            var paragraphTwo = resoltution.CreateOperativeParagraph();
            var result = resoltution.FindOperativeParagraph(paragraphOne.OperativeParagraphId);
            Assert.NotNull(result);
            Assert.AreEqual(paragraphOne, result);
        }

        [Test]
        public void FindSecondLevelOperativeParagraph()
        {
            var resolution = new Resolution();
            var paragraphOne = resolution.CreateOperativeParagraph();
            var paragraphOneSubone = resolution.CreateChildParagraph(paragraphOne);
            var result = resolution.FindOperativeParagraph(paragraphOneSubone.OperativeParagraphId);
            Assert.NotNull(result);
            Assert.AreEqual(paragraphOneSubone, result);
        }

        [Test]
        public void TestGetFirstLevelPath()
        {
            var resolution = new Resolution();
            var paragraphOne = resolution.CreateOperativeParagraph();
            var path = resolution.GetOperativeParagraphPath(paragraphOne.OperativeParagraphId);
            Assert.AreEqual(1, path.Count);
            Assert.AreEqual(paragraphOne, path[0]);
        }

        [Test]
        public void TestGetSecondLevelPath()
        {
            var resolution = new Resolution();
            var paragraphOne = resolution.CreateOperativeParagraph("1");
            var paragraphOneSubone = resolution.CreateChildParagraph(paragraphOne, "1.a");
            var path = resolution.GetOperativeParagraphPath(paragraphOneSubone.OperativeParagraphId);
            Assert.AreEqual(2, path.Count);
            Assert.AreEqual(paragraphOne, path[0]);
            Assert.AreEqual(paragraphOneSubone, path[1]);
        }

        [Test]
        public void TestGetThirdLevelPath()
        {
            var resolution = new Resolution();
            var paragraphOne = resolution.CreateOperativeParagraph("1");
            var paragraphOneSubone = resolution.CreateChildParagraph(paragraphOne, "1.a");
            var paragraphLevelThree = resolution.CreateChildParagraph(paragraphOneSubone, "1.a.i");
            var path = resolution.GetOperativeParagraphPath(paragraphLevelThree.OperativeParagraphId);
            Assert.AreEqual(3, path.Count);
            Assert.AreEqual(paragraphOne, path[0]);
            Assert.AreEqual(paragraphOneSubone, path[1]);
            Assert.AreEqual(paragraphLevelThree, path[2]);
        }

    }
}
