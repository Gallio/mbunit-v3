// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
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

using System;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Linq;
using Gallio.Common.Markup;
using Gallio.Common.Policies;
using Gallio.Reports;
using Gallio.Reports.Vtl;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.Runtime;
using Gallio.Framework;
using Gallio.Common.Reflection;
using Gallio.Runner.Reports;
using Gallio.Tests;
using MbUnit.Framework;
using Rhino.Mocks;
using Rhino.Mocks.Constraints;
using Gallio.Common.Collections;
using Gallio.Runner.Reports.Schema;
using NVelocity.App;
using NVelocity.Runtime;
using System.Reflection;
using Gallio.Common.Markup.Tags;
using Gallio.Model.Schema;
using System.Collections.Generic;
using Gallio.Model;

namespace Gallio.Tests.Reports.Vtl
{
    [TestFixture]
    [TestsOn(typeof(TestStepRunNode))]
    public class TestStepRunNodeTest
    {
        [Test]
        [ExpectedArgumentNullException]
        public void Constructs_with_null_run_should_throw_exception()
        {
            new TestStepRunNode(null, null, 0);
        }

        [Test]
        public void Constructs_root_element() // i.e. without any parent
        {
            var run = CreateFakeTestStepRun("123", true, TestOutcome.Passed);
            var node = new TestStepRunNode(run, null, 0);
            Assert.AreSame(run, node.Run);
            Assert.IsNull(node.Parent);
            Assert.AreEqual(0, node.Index);
            Assert.IsEmpty(node.Children);
            Assert.AreEqual(1, node.Count);
        }

        [Test]
        public void Constructs_not_root_element()
        {
            var runParent = CreateFakeTestStepRun("123", false, TestOutcome.Passed);
            var parent = new TestStepRunNode(runParent, null, 0);
            var run = CreateFakeTestStepRun("456", true, TestOutcome.Passed);
            var node = new TestStepRunNode(run, parent, 1);
            Assert.AreSame(run, node.Run);
            Assert.AreSame(parent, node.Parent);
            Assert.AreEqual(1, node.Index);
            Assert.IsEmpty(node.Children);
            Assert.AreEqual(1, node.Count);
        }

        [Test]
        public void BuildTreeFromRoot_with_null_root()
        {
            var tree = TestStepRunNode.BuildTreeFromRoot(null);
            Assert.AreEqual(1, tree.Count);
        }

        private TestStepRun CreateFakeTestStepRun(string id, bool isTestCase, TestOutcome outcome, params TestStepRun[] children)
        {
            var step = new TestStepData(id, "Name-" + id, "FullName-" + id, "Test-" + id);
            step.IsTestCase = isTestCase;
            var run = new TestStepRun(step);
            run.Result = new TestResult() { Outcome = outcome };
            run.Children.AddRange(children);
            return run;
        }

        private TestStepRunNode BuildFakeTree()
        {
            // Root 
            //   +- Child1
            //   +- Child2
            //   +- Child3
            //       +- Child31
            //       +- Child32
            //           +- Child321
            //           +- Child322
            //           +- Child323
            //           +- Child324
            //       +- Child33
            //           +- Child331
            //           +- Child332
            //           +- Child333

            var child1 = CreateFakeTestStepRun("1", true, TestOutcome.Passed);
            var child2 = CreateFakeTestStepRun("2", true, TestOutcome.Passed);
            var child331 = CreateFakeTestStepRun("331", true, TestOutcome.Passed);
            var child332 = CreateFakeTestStepRun("332", true, TestOutcome.Pending);
            var child333 = CreateFakeTestStepRun("333", true, TestOutcome.Passed);
            var child33 = CreateFakeTestStepRun("33", false, TestOutcome.Passed, child331, child332, child333);
            var child321 = CreateFakeTestStepRun("321", true, TestOutcome.Inconclusive);
            var child322 = CreateFakeTestStepRun("322", true, TestOutcome.Passed);
            var child323 = CreateFakeTestStepRun("323", true, TestOutcome.Failed);
            var child324 = CreateFakeTestStepRun("324", true, TestOutcome.Ignored);
            var child32 = CreateFakeTestStepRun("32", false, TestOutcome.Failed, child321, child322, child323, child324);
            var child31 = CreateFakeTestStepRun("31", true, TestOutcome.Passed);
            var child3 = CreateFakeTestStepRun("3", false, TestOutcome.Failed, child31, child32, child33);
            var root = CreateFakeTestStepRun("Root", false, TestOutcome.Failed, child1, child2, child3);
            return TestStepRunNode.BuildTreeFromRoot(root);
        }

        [Test]
        public void Count()
        {
            TestStepRunNode tree = BuildFakeTree();
            Assert.AreEqual(14, tree.Count);
        }

        [Test]
        public void GetSummaryChildren_condensed()
        {
            TestStepRunNode tree = BuildFakeTree();
            IEnumerable<TestStepRunNode> nodes = tree.GetSummaryChildren(true);
            Assert.AreElementsEqual(new[] { "3" }, nodes.Select(x => x.Run.Step.Id));
            nodes = nodes.ElementAt(0).GetSummaryChildren(true);
            Assert.AreElementsEqual(new[] { "32" }, nodes.Select(x => x.Run.Step.Id));
            nodes = nodes.ElementAt(0).GetSummaryChildren(true);
            Assert.IsEmpty(nodes);
        }

        [Test]
        public void GetSummaryChildren_not_condensed()
        {
            TestStepRunNode tree = BuildFakeTree();
            IEnumerable<TestStepRunNode> nodes = tree.GetSummaryChildren(false);
            Assert.AreElementsEqual(new[] { "3" }, nodes.Select(x => x.Run.Step.Id));
            nodes = nodes.ElementAt(0).GetSummaryChildren(false);
            Assert.AreElementsEqual(new[] { "32", "33" }, nodes.Select(x => x.Run.Step.Id));
            nodes = nodes.ElementAt(0).GetSummaryChildren(false);
            Assert.IsEmpty(nodes);
        }

        [Test]
        public void GetDetailsChildren_condensed()
        {
            TestStepRunNode tree = BuildFakeTree();
            IEnumerable<TestStepRunNode> nodes = tree.GetDetailsChildren(true);
            Assert.AreElementsEqual(new[] { "3" }, nodes.Select(x => x.Run.Step.Id));
            nodes = nodes.ElementAt(0).GetDetailsChildren(true);
            Assert.AreElementsEqual(new[] { "32" }, nodes.Select(x => x.Run.Step.Id));
            nodes = nodes.ElementAt(0).GetDetailsChildren(true);
            Assert.AreElementsEqual(new[] { "321", "323", "324" }, nodes.Select(x => x.Run.Step.Id));
        }

        [Test]
        public void GetDetailsChildren_not_condensed()
        {
            TestStepRunNode tree = BuildFakeTree();
            IEnumerable<TestStepRunNode> nodes = tree.GetDetailsChildren(false);
            Assert.AreElementsEqual(new[] { "1", "2", "3" }, nodes.Select(x => x.Run.Step.Id));
            nodes = nodes.ElementAt(2).GetDetailsChildren(false);
            Assert.AreElementsEqual(new[] { "31", "32", "33" }, nodes.Select(x => x.Run.Step.Id));
            nodes = nodes.ElementAt(1).GetDetailsChildren(false);
            Assert.AreElementsEqual(new[] { "321", "322", "323", "324" }, nodes.Select(x => x.Run.Step.Id));
        }

        [Test]
        public void GetNavigatorChildren()
        {
            TestStepRunNode tree = BuildFakeTree();
            IEnumerable<TestStepRunNode> nodes = tree.GetNavigatorChildren();
            Assert.AreElementsEqual(new[] { "321", "323", "324", "332" }, nodes.Select(x => x.Run.Step.Id));
        }

        [Test]
        public void GetSelfAndAncestorIds()
        {
            TestStepRunNode tree = BuildFakeTree();
            var child321 = tree.Children[2].Children[1].Children[0];
            Assert.AreElementsEqual(new[] { "Test-321", "Test-32", "Test-3", "Test-Root" }, child321.GetSelfAndAncestorIds());
        }

        [Test]
        [Row(0, 1, false)]
        [Row(123, 1, true)]
        [Row(999, 1, true)]
        [Row(1000, 1, true)]
        [Row(1000, 2, false)]
        [Row(1001, 2, true)]
        [Row(3500, 4, true)]
        [Row(3500, 5, false)]
        public void IsVisibleInPage(int index, int pageIndex, bool expectedVisible)
        {
            var run = CreateFakeTestStepRun("123", true, TestOutcome.Passed);
            var node = new TestStepRunNode(run, null, index);
            bool actualVisible = node.IsVisibleInPage(pageIndex, 1000);
            Assert.AreEqual(expectedVisible, actualVisible);
        }
    }
}