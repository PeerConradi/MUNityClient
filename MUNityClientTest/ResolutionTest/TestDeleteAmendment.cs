﻿using NUnit.Framework;
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
            var operativeParagraph = resolution.OperativeSection.CreateOperativeParagraph();
            var amendment = resolution.OperativeSection.CreateDeleteAmendment(operativeParagraph);
            Assert.NotNull(amendment);
            Assert.Contains(amendment, resolution.OperativeSection.DeleteAmendments);
            Assert.AreEqual(1, resolution.OperativeSection.DeleteAmendments.Count);
        }

        [Test]
        public void TestRemoveAmendment()
        {
            var resolution = new Resolution();
            var operativeParagraph = resolution.OperativeSection.CreateOperativeParagraph();
            var amendment = resolution.OperativeSection.CreateDeleteAmendment(operativeParagraph);
            Assert.NotNull(amendment);
            Assert.Contains(amendment, resolution.OperativeSection.DeleteAmendments);
            resolution.OperativeSection.RemoveAmendment(amendment);
            Assert.Contains(operativeParagraph, resolution.OperativeSection.Paragraphs);
        }

        [Test]
        public void TestApplyDeleteAmendment()
        {
            var resolution = new Resolution();
            var operativeParagraph = resolution.OperativeSection.CreateOperativeParagraph();
            var amendment = resolution.OperativeSection.CreateDeleteAmendment(operativeParagraph);
            amendment.Apply(resolution.OperativeSection);
            Assert.IsFalse(resolution.OperativeSection.Paragraphs.Contains(operativeParagraph));
            Assert.IsFalse(resolution.OperativeSection.DeleteAmendments.Contains(amendment));
        }

        [Test]
        public void TestApplyRemovesAllOtherAmendments()
        {
            var resolution = new Resolution();
            var operativeParagraph = resolution.OperativeSection.CreateOperativeParagraph();
            var operativeParagraphTwo = resolution.OperativeSection.CreateOperativeParagraph();
            var deleteAmendment = resolution.OperativeSection.CreateDeleteAmendment(operativeParagraph.OperativeParagraphId);
            var changeAmendment = resolution.OperativeSection.CreateChangeAmendment(operativeParagraph.OperativeParagraphId);
            var moveAmendment = resolution.OperativeSection.CreateMoveAmendment(operativeParagraph.OperativeParagraphId, 1);
            deleteAmendment.Apply(resolution.OperativeSection);
            Assert.IsFalse(resolution.OperativeSection.ChangeAmendments.Any(n => n.TargetSectionId == operativeParagraph.OperativeParagraphId));
            Assert.IsFalse(resolution.OperativeSection.DeleteAmendments.Any(n => n.TargetSectionId == operativeParagraph.OperativeParagraphId));
            Assert.IsFalse(resolution.OperativeSection.MoveAmendments.Any(n => n.TargetSectionId == operativeParagraph.OperativeParagraphId));
        }

        [Test]
        public void TestDenyDeleteAmendment()
        {
            var resolution = new Resolution();
            var operativeParagraph = resolution.OperativeSection.CreateOperativeParagraph();
            var deleteAmendment = resolution.OperativeSection.CreateDeleteAmendment(operativeParagraph);
            var deleteAmendmentTwo = resolution.OperativeSection.CreateDeleteAmendment(operativeParagraph);
            deleteAmendment.Deny(resolution.OperativeSection);
            Assert.IsFalse(resolution.OperativeSection.DeleteAmendments.Any(n => n.TargetSectionId == operativeParagraph.OperativeParagraphId));
        }
    }
}
