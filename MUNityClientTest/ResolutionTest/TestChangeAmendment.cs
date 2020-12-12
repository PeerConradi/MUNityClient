using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using MUNityClient.Models.Resolution;
using MUNityClient.Extensions.ResolutionExtensions;

namespace MUNityClientTest.ResolutionTest
{
    class TestChangeAmendment
    {
        [Test]
        public void TestCreateInstance()
        {
            var resolution = new Resolution();
            var operativeParagraph = resolution.CreateOperativeParagraph();
            var changeAmendmet = resolution.CreateChangeAmendment(operativeParagraph);
            Assert.NotNull(changeAmendmet);
            Assert.Contains(changeAmendmet, resolution.OperativeSection.ChangeAmendments);
            Assert.AreEqual(1, resolution.OperativeSection.ChangeAmendments.Count);
        }

        [Test]
        public void TestRemoveAmendment()
        {
            var resolution = new Resolution();
            var operativeParagraph = resolution.CreateOperativeParagraph();
            var changeAmendment = resolution.CreateChangeAmendment(operativeParagraph);
            resolution.RemoveAmendment(changeAmendment);
            Assert.IsFalse(resolution.OperativeSection.ChangeAmendments.Contains(changeAmendment));
        }

        [Test]
        public void TestApplyChangeAmendment()
        {
            var resolution = new Resolution();
            var operativeParagraph = resolution.CreateOperativeParagraph("Original Text");
            var changeAmendment = resolution.CreateChangeAmendment(operativeParagraph, "New Text");
            Assert.AreEqual("Original Text", operativeParagraph.Text);
            changeAmendment.Apply(resolution);
            Assert.AreEqual("New Text", operativeParagraph.Text);
            Assert.IsFalse(resolution.OperativeSection.ChangeAmendments.Contains(changeAmendment));
        }

        [Test]
        public void TestDenyChangeAmendment()
        {
            var resolution = new Resolution();
            var operativeParagraph = resolution.CreateOperativeParagraph("Original Text");
            var changeAmendment = resolution.CreateChangeAmendment(operativeParagraph, "New Text");
            Assert.AreEqual("Original Text", operativeParagraph.Text);
            changeAmendment.Deny(resolution);
            Assert.AreEqual("Original Text", operativeParagraph.Text);
            Assert.IsFalse(resolution.OperativeSection.ChangeAmendments.Contains(changeAmendment));
        }
    }
}
