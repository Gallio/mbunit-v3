// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan de Halleux
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Gallio.Icarus.Controls;

using MbUnit.Framework;
using Gallio.Model;
using Gallio.Runner.Reports;
using Gallio.Model.Serialization;
using System.Windows.Forms;
using System.Drawing;

namespace Gallio.Icarus.Controls.Tests
{
    [TestFixture]
    public class TestTreeNodeTest
    {
        private TestTreeNode testTreeNode;

        [SetUp]
        public void SetUp()
        {
            testTreeNode = new TestTreeNode("text", "name", TestKinds.Test);
        }

        [Test]
        public void Name_Test()
        {
            Assert.AreEqual("name", testTreeNode.Name);
        }

        [Test]
        public void NodeType_Test()
        {
            Assert.AreEqual(TestKinds.Test, testTreeNode.NodeType);
        }

        [Test]
        public void TestStatus_Test()
        {
            Assert.AreEqual(TestStatus.Skipped, testTreeNode.TestStatus);
            testTreeNode.TestStatus = TestStatus.Passed;
            Assert.AreEqual(TestStatus.Passed, testTreeNode.TestStatus);
            testTreeNode.TestStatus = TestStatus.Failed;
            Assert.AreEqual(TestStatus.Failed, testTreeNode.TestStatus);
            testTreeNode.TestStatus = TestStatus.Inconclusive;
            Assert.AreEqual(TestStatus.Inconclusive, testTreeNode.TestStatus);
        }

        [Test]
        public void SourceCodeAvailable_Test()
        {
            Assert.IsFalse(testTreeNode.SourceCodeAvailable);
            testTreeNode.SourceCodeAvailable = true;
            Assert.IsTrue(testTreeNode.SourceCodeAvailable);
        }

        [Test]
        public void IsTest_Test()
        {
            Assert.IsFalse(testTreeNode.IsTest);
            testTreeNode.IsTest = true;
            Assert.IsTrue(testTreeNode.IsTest);
        }

        [Test]
        public void NodeTypeIcon_Test()
        {
            Image image = testTreeNode.NodeTypeIcon;
            // Need some way to compare images!
            //Assert.AreEqual(global::Gallio.Icarus.Properties.Resources.Test, image);
        }

        [Test]
        public void TestStatusIcon_Test()
        {
            Assert.IsNull(testTreeNode.TestStatusIcon);
            testTreeNode.TestStatus = TestStatus.Passed;
            Image image = testTreeNode.TestStatusIcon;
            // Need some way to compare images!
            //Assert.AreEqual(global::Gallio.Icarus.Properties.Resources.tick, image);
        }

        [Test]
        public void Find_Test()
        {
            TestTreeNode child1 = new TestTreeNode("child1", "child1", "child1");
            testTreeNode.Nodes.Add(child1);
            TestTreeNode child2 = new TestTreeNode("child2", "child2", "child2");
            child1.Nodes.Add(child2);
            Assert.AreEqual(1, testTreeNode.Find("child2", true).Count);
        }

        [Test]
        public void UpdateStateOfRelatedNodes_Test()
        {
            TestTreeNode child1 = new TestTreeNode("child1", "child1", "child1");
            testTreeNode.Nodes.Add(child1);
            TestTreeNode child2 = new TestTreeNode("child2", "child2", "child2");
            child1.Nodes.Add(child2);
            child2.CheckState = CheckState.Unchecked;
            TestTreeNode child3 = new TestTreeNode("child3", "child3", "child3");
            child1.Nodes.Add(child3);
            child3.CheckState = CheckState.Unchecked;
            Assert.AreEqual(CheckState.Checked, testTreeNode.CheckState);
            child3.UpdateStateOfRelatedNodes();
            Assert.AreEqual(CheckState.Unchecked, testTreeNode.CheckState);
            Assert.AreEqual(CheckState.Unchecked, child1.CheckState);
        }

        [Test]
        public void AddTestStepRun_Test()
        {
            Assert.AreEqual(0, testTreeNode.TestStepRuns.Count);
            TestStepRun testStepRun = new TestStepRun(new TestStepData("id", "name", "fullName", "testId"));
            testTreeNode.AddTestStepRun(testStepRun);
            Assert.AreEqual(1, testTreeNode.TestStepRuns.Count);
            Assert.AreEqual(testStepRun, testTreeNode.TestStepRuns[0]);
        }

        [Test]
        public void ClearTestStepRuns_Test()
        {
            Assert.AreEqual(0, testTreeNode.TestStepRuns.Count);
            testTreeNode.AddTestStepRun(new TestStepRun(new TestStepData("id", "name", "fullName", "testId")));
            Assert.AreEqual(1, testTreeNode.TestStepRuns.Count);
            testTreeNode.ClearTestStepRuns();
            Assert.AreEqual(0, testTreeNode.TestStepRuns.Count);
        }

        [Test]
        public void SiblingTestStatus_Test()
        {
            TestTreeNode child1 = new TestTreeNode("child1", "child1", "child1");
            testTreeNode.Nodes.Add(child1);
            TestTreeNode child2 = new TestTreeNode("child2", "child2", "child2");
            testTreeNode.Nodes.Add(child2);
            TestTreeNode child3 = new TestTreeNode("child3", "child3", "child3");
            testTreeNode.Nodes.Add(child3);
            child1.TestStatus = TestStatus.Passed;
            Assert.AreEqual(TestStatus.Passed, child1.TestStatus);
            Assert.AreEqual(TestStatus.Skipped, child2.TestStatus);
            Assert.AreEqual(TestStatus.Skipped, child3.TestStatus);
            Assert.AreEqual(TestStatus.Passed, testTreeNode.TestStatus);
            child2.TestStatus = TestStatus.Inconclusive;
            Assert.AreEqual(TestStatus.Passed, child1.TestStatus);
            Assert.AreEqual(TestStatus.Inconclusive, child2.TestStatus);
            Assert.AreEqual(TestStatus.Skipped, child3.TestStatus);
            Assert.AreEqual(TestStatus.Inconclusive, testTreeNode.TestStatus);
            child3.TestStatus = TestStatus.Failed;
            Assert.AreEqual(TestStatus.Passed, child1.TestStatus);
            Assert.AreEqual(TestStatus.Inconclusive, child2.TestStatus);
            Assert.AreEqual(TestStatus.Failed, child3.TestStatus);
            Assert.AreEqual(TestStatus.Failed, testTreeNode.TestStatus);
            child2.TestStatus = TestStatus.Passed;
            Assert.AreEqual(TestStatus.Passed, child1.TestStatus);
            Assert.AreEqual(TestStatus.Passed, child2.TestStatus);
            Assert.AreEqual(TestStatus.Failed, child3.TestStatus);
            Assert.AreEqual(TestStatus.Failed, testTreeNode.TestStatus);
        }
    }
}