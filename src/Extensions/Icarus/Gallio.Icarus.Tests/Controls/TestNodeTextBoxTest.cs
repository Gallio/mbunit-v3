using System.Drawing;
using Aga.Controls.Tree;
using Aga.Controls.Tree.NodeControls;
using Gallio.Icarus.Controls;
using Gallio.Icarus.Models;
using Gallio.Model;
using MbUnit.Framework;

namespace Gallio.Icarus.Tests.Controls
{
    [Category("Controls"), TestsOn(typeof(TestNodeTextBox))]
    public class TestNodeTextBoxTest
    {
        [Test]
        public void If_test_status_is_Passed_text_should_be_passed_color()
        {
            var passedColor = Color.Firebrick;
            var nodeTextBox = new TestTestNodeTextBox
            {
                PassedColor = passedColor
            };
            var treeNode = new TestTreeNode("id", "text")
            {
                TestStatus = TestStatus.Passed
            };
            var eventArgs = new DrawEventArgs(new TreeNodeAdv(treeNode), 
                nodeTextBox, new DrawContext(), "text");

            nodeTextBox.TestOnDrawText(eventArgs);

            Assert.AreEqual(passedColor, eventArgs.TextColor);
        }

        [Test]
        public void If_test_status_is_Failed_text_should_be_failed_color()
        {
            var failedColor = Color.DarkViolet;
            var nodeTextBox = new TestTestNodeTextBox
            {
                FailedColor = failedColor
            };
            var treeNode = new TestTreeNode("id", "text")
            {
                TestStatus = TestStatus.Failed
            };
            var eventArgs = new DrawEventArgs(new TreeNodeAdv(treeNode),
                nodeTextBox, new DrawContext(), "text");

            nodeTextBox.TestOnDrawText(eventArgs);

            Assert.AreEqual(failedColor, eventArgs.TextColor);
        }

        [Test]
        public void If_test_status_is_Skipped_text_should_be_failed_color()
        {
            var skippedColor = Color.Khaki;
            var nodeTextBox = new TestTestNodeTextBox
            {
                SkippedColor = skippedColor
            };
            var treeNode = new TestTreeNode("id", "text")
            {
                TestStatus = TestStatus.Skipped
            };
            var eventArgs = new DrawEventArgs(new TreeNodeAdv(treeNode),
                nodeTextBox, new DrawContext(), "text");

            nodeTextBox.TestOnDrawText(eventArgs);

            Assert.AreEqual(skippedColor, eventArgs.TextColor);
        }

        [Test]
        public void If_test_status_is_Inconclusive_text_should_be_inconclusive_color()
        {
            var inconclusiveColor = Color.Magenta;
            var nodeTextBox = new TestTestNodeTextBox
            {
                InconclusiveColor = inconclusiveColor
            };
            var treeNode = new TestTreeNode("id", "text")
            {
                TestStatus = TestStatus.Inconclusive
            };
            var eventArgs = new DrawEventArgs(new TreeNodeAdv(treeNode),
                nodeTextBox, new DrawContext(), "text");

            nodeTextBox.TestOnDrawText(eventArgs);

            Assert.AreEqual(inconclusiveColor, eventArgs.TextColor);
        }

        [Test]
        public void If_node_is_not_test_node_then_nothing_should_happen()
        {
            var color = Color.SandyBrown;
            var nodeTextBox = new TestTestNodeTextBox();
            var eventArgs = new DrawEventArgs(new TreeNodeAdv(new Node("text")), 
                nodeTextBox, new DrawContext(), "text")
            {
                TextColor = color
            };

            nodeTextBox.TestOnDrawText(eventArgs);

            Assert.AreEqual(color, eventArgs.TextColor);
        }

        [Test]
        public void DrawTextMustBeFired_returns_true()
        {
            var nodeTextBox = new TestTestNodeTextBox();

            var drawTextMustBeFired = nodeTextBox.GetDrawTextMustBeFired();

            Assert.IsTrue(drawTextMustBeFired);
        }

        private class TestTestNodeTextBox : TestNodeTextBox
        {
            public void TestOnDrawText(DrawEventArgs args)
            {
                base.OnDrawText(args);
            }

            public bool GetDrawTextMustBeFired()
            {
                return base.DrawTextMustBeFired(new TreeNodeAdv(null));
            }
        }
    }
}
