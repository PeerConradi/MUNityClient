using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using MUNityClient.Models.Resolution;
using MUNityClient.Extensions.ResolutionExtensions;

namespace MUNityClientTest.ResolutionTest
{
    public class TestAddAmendments
    {
        [Test]
        public void TestCreateInstance()
        {
            var resolution = new Resolution();
            var paragraphOne = resolution.OperativeSection.CreateOperativeParagraph();
            var amendment = resolution.OperativeSection.CreateAddAmendment(1, "New Paragraph");
            Assert.NotNull(amendment);
            Assert.AreEqual(1, resolution.OperativeSection.AddAmendments.Count);
            Assert.AreEqual(2, resolution.OperativeSection.Paragraphs.Count);
            Assert.AreEqual(paragraphOne, resolution.OperativeSection.Paragraphs[0]);
            Assert.IsTrue(resolution.OperativeSection.Paragraphs[1].IsVirtual);
            Assert.Contains(amendment, resolution.OperativeSection.AddAmendments);
        }

        [Test]
        public void TestApplyAmendment()
        {
            var resolution = new Resolution();
            var paragraphOne = resolution.OperativeSection.CreateOperativeParagraph();
            var amendment = resolution.OperativeSection.CreateAddAmendment(1, "New Paragraph");
            amendment.Apply(resolution.OperativeSection);
            Assert.AreEqual(2, resolution.OperativeSection.Paragraphs.Count);
            Assert.AreEqual(paragraphOne, resolution.OperativeSection.Paragraphs[0]);
            Assert.IsFalse(resolution.OperativeSection.Paragraphs[1].IsVirtual);
            Assert.IsFalse(resolution.OperativeSection.AddAmendments.Contains(amendment));
            Assert.AreEqual("New Paragraph", resolution.OperativeSection.Paragraphs[1].Text);
        }

        [Test]
        public void TestDeleteAddAmendment()
        {
            var resolution = new Resolution();
            var paragraphOne = resolution.OperativeSection.CreateOperativeParagraph();
            var amendment = resolution.OperativeSection.CreateAddAmendment(1, "New Paragraph");
            resolution.OperativeSection.RemoveAmendment(amendment);
            Assert.AreEqual(1, resolution.OperativeSection.Paragraphs.Count);
        }
    }
}
