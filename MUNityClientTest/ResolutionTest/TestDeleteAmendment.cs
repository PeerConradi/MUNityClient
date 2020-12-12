using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using MUNityClient.Models.Resolution;
using System.Linq;
using MUNityClient.Extensions.ResolutionExtensions;

namespace MUNityClientTest.ResolutionTest
{
    public class TestDeleteAmendment
    {
        [Test]
        public void TestCreateInstance()
        {
            var resolution = new Resolution();
            var operativeParagraph = resolution.CreateOperativeParagraph();
            var amendment = resolution.CreateDeleteAmendment(operativeParagraph);
            Assert.NotNull(amendment);
            Assert.Contains(amendment, resolution.OperativeSection.DeleteAmendments);
            Assert.AreEqual(1, resolution.OperativeSection.DeleteAmendments.Count);
        }

        [Test]
        public void TestRemoveAmendment()
        {
            var resolution = new Resolution();
            var operativeParagraph = resolution.CreateOperativeParagraph();
            var amendment = resolution.CreateDeleteAmendment(operativeParagraph);
            Assert.NotNull(amendment);
            Assert.Contains(amendment, resolution.OperativeSection.DeleteAmendments);
            resolution.RemoveAmendment(amendment);
            Assert.Contains(operativeParagraph, resolution.OperativeSection.Paragraphs);
        }

        [Test]
        public void TestApplyDeleteAmendment()
        {
            var resolution = new Resolution();
            var operativeParagraph = resolution.CreateOperativeParagraph();
            var amendment = resolution.CreateDeleteAmendment(operativeParagraph);
            amendment.Apply(resolution);
            Assert.IsFalse(resolution.OperativeSection.Paragraphs.Contains(operativeParagraph));
            Assert.IsFalse(resolution.OperativeSection.DeleteAmendments.Contains(amendment));
        }

        [Test]
        public void TestApplyRemovesAllOtherAmendments()
        {
            var resolution = new Resolution();
            var operativeParagraph = resolution.CreateOperativeParagraph();
            var operativeParagraphTwo = resolution.CreateOperativeParagraph();
            var deleteAmendment = resolution.CreateDeleteAmendment(operativeParagraph.OperativeParagraphId);
            var changeAmendment = resolution.CreateChangeAmendment(operativeParagraph.OperativeParagraphId);
            var moveAmendment = resolution.CreateMoveAmendment(operativeParagraph.OperativeParagraphId, 1);
            deleteAmendment.Apply(resolution);
            Assert.IsFalse(resolution.OperativeSection.ChangeAmendments.Any(n => n.TargetSectionId == operativeParagraph.OperativeParagraphId));
            Assert.IsFalse(resolution.OperativeSection.DeleteAmendments.Any(n => n.TargetSectionId == operativeParagraph.OperativeParagraphId));
            Assert.IsFalse(resolution.OperativeSection.MoveAmendments.Any(n => n.TargetSectionId == operativeParagraph.OperativeParagraphId));
        }

        [Test]
        public void TestDenyDeleteAmendment()
        {
            var resolution = new Resolution();
            var operativeParagraph = resolution.CreateOperativeParagraph();
            var deleteAmendment = resolution.CreateDeleteAmendment(operativeParagraph);
            var deleteAmendmentTwo = resolution.CreateDeleteAmendment(operativeParagraph);
            deleteAmendment.Deny(resolution);
            Assert.IsFalse(resolution.OperativeSection.DeleteAmendments.Any(n => n.TargetSectionId == operativeParagraph.OperativeParagraphId));
        }
    }
}
