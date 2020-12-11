using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using MUNityClient.Models.Resolution;
using System.Linq;

namespace MUNityClientTest.ResolutionTest
{
    public class TestMoveAmendment
    {
        [Test]
        public void TestCreateInstance()
        {
            var resolution = new Resolution();
            var paragraphOne = resolution.CreateOperativeParagraph("Paragraph One");
            var paragraphTwo = resolution.CreateOperativeParagraph("Paragraph Two");
            var moveAmendment = resolution.CreateMoveAmendment(paragraphOne, 1);
            Assert.NotNull(moveAmendment);
            Assert.Contains(moveAmendment, resolution.OperativeSection.MoveAmendments);
            Assert.AreEqual(3, resolution.OperativeSection.Paragraphs.Count);
            Assert.AreEqual(2, resolution.OperativeSection.Paragraphs.Count(n => !n.IsVirtual));
            Assert.AreEqual(paragraphOne, resolution.OperativeSection.Paragraphs[0]);
            Assert.AreEqual(paragraphTwo, resolution.OperativeSection.Paragraphs[1]);
        }

        [Test]
        public void TestRemoveAmendment()
        {
            var resolution = new Resolution();
            var paragraphOne = resolution.CreateOperativeParagraph("Paragraph One");
            var paragraphTwo = resolution.CreateOperativeParagraph("Paragraph Two");
            var moveAmendment = resolution.CreateMoveAmendment(paragraphOne, 1);
            resolution.RemoveAmendment(moveAmendment);
            Assert.IsFalse(resolution.OperativeSection.MoveAmendments.Contains(moveAmendment));
            Assert.AreEqual(2, resolution.OperativeSection.Paragraphs.Count);
            Assert.AreEqual(2, resolution.OperativeSection.Paragraphs.Count(n => !n.IsVirtual));
        }

        [Test]
        public void TestApplyAmendment()
        {
            var resolution = new Resolution();
            var paragraphOne = resolution.CreateOperativeParagraph("Paragraph One");
            var paragraphTwo = resolution.CreateOperativeParagraph("Paragraph Two");
            var moveAmendment = resolution.CreateMoveAmendment(paragraphOne, 1);
            var success = moveAmendment.Apply(resolution);
            Assert.IsTrue(success);
            Assert.AreEqual(2, resolution.OperativeSection.Paragraphs.Count);
            Assert.AreEqual(2, resolution.OperativeSection.Paragraphs.Count(n => !n.IsVirtual));
            var firstParagraph = resolution.OperativeSection.Paragraphs[0];
            var secondParagraph = resolution.OperativeSection.Paragraphs[1];
            // Paragraph should have moved up
            Assert.AreEqual("Paragraph Two", firstParagraph.Text);
            Assert.AreEqual("Paragraph One", secondParagraph.Text);
            // Alle Daten sollten nun mit dem Platzhalter ausgetauscht sein.

        }
    }
}
