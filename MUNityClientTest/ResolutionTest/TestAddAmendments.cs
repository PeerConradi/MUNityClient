using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using MUNityClient.Models.Resolution;

namespace MUNityClientTest.ResolutionTest
{
    public class TestAddAmendments
    {
        [Test]
        public void TestCreateInstance()
        {
            var resolution = new Resolution();
            var paragraphOne = resolution.CreateOperativeParagraph();
            var amendment = resolution.CreateAddAmendment(1, "New Paragraph");
            Assert.NotNull(amendment);
            Assert.AreEqual(2, resolution.OperativeSection.Paragraphs.Count);
            Assert.AreEqual(paragraphOne, resolution.OperativeSection.Paragraphs[0]);
            Assert.IsTrue(resolution.OperativeSection.Paragraphs[1].IsVirtual);
            Assert.Contains(amendment, resolution.OperativeSection.AddAmendments);
            Assert.AreEqual(1, amendment.Position);
        }

        [Test]
        public void TestApplyAmendment()
        {
            var resolution = new Resolution();
            var paragraphOne = resolution.CreateOperativeParagraph();
            var amendment = resolution.CreateAddAmendment(1, "New Paragraph");
            amendment.Apply(resolution);
            Assert.AreEqual(paragraphOne, resolution.OperativeSection.Paragraphs[0]);
            Assert.IsFalse(resolution.OperativeSection.Paragraphs[1].IsVirtual);
            Assert.IsFalse(resolution.OperativeSection.AddAmendments.Contains(amendment));
        }
    }
}
