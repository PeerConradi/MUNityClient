﻿using NUnit.Framework;
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
            var operativeParagraph = resolution.OperativeSection.CreateOperativeParagraph();
            var changeAmendmet = resolution.OperativeSection.CreateChangeAmendment(operativeParagraph);
            Assert.NotNull(changeAmendmet);
            Assert.Contains(changeAmendmet, resolution.OperativeSection.ChangeAmendments);
            Assert.AreEqual(1, resolution.OperativeSection.ChangeAmendments.Count);
        }

        [Test]
        public void TestRemoveAmendment()
        {
            var resolution = new Resolution();
            var operativeParagraph = resolution.OperativeSection.CreateOperativeParagraph();
            var changeAmendment = resolution.OperativeSection.CreateChangeAmendment(operativeParagraph);
            resolution.OperativeSection.RemoveAmendment(changeAmendment);
            Assert.IsFalse(resolution.OperativeSection.ChangeAmendments.Contains(changeAmendment));
        }

        [Test]
        public void TestApplyChangeAmendment()
        {
            var resolution = new Resolution();
            var operativeParagraph = resolution.OperativeSection.CreateOperativeParagraph("Original Text");
            var changeAmendment = resolution.OperativeSection.CreateChangeAmendment(operativeParagraph, "New Text");
            Assert.AreEqual("Original Text", operativeParagraph.Text);
            changeAmendment.Apply(resolution.OperativeSection);
            Assert.AreEqual("New Text", operativeParagraph.Text);
            Assert.IsFalse(resolution.OperativeSection.ChangeAmendments.Contains(changeAmendment));
        }

        [Test]
        public void TestDenyChangeAmendment()
        {
            var resolution = new Resolution();
            var operativeParagraph = resolution.OperativeSection.CreateOperativeParagraph("Original Text");
            var changeAmendment = resolution.OperativeSection.CreateChangeAmendment(operativeParagraph, "New Text");
            Assert.AreEqual("Original Text", operativeParagraph.Text);
            changeAmendment.Deny(resolution.OperativeSection);
            Assert.AreEqual("Original Text", operativeParagraph.Text);
            Assert.IsFalse(resolution.OperativeSection.ChangeAmendments.Contains(changeAmendment));
        }
    }
}
