// Copyright 2008 MbUnit Project - http://www.mbunit.com/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
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
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Gallio.Collections;
using Gallio.Model;
using Gallio.Model.Execution;
using Gallio.Model.Filters;
using Gallio.Reflection;
using MbUnit.Framework;

namespace Gallio.Tests.Model.Execution
{
    [TestFixture]
    [TestsOn(typeof(TestCommandFactory))]
    public class TestCommandFactoryTest : BaseUnitTest
    {
        private TestModel model;
        private ITestCommand rootCommand;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            model = new TestModel(new TestPackage(new TestPackageConfig(), Reflector.ReflectionPolicy));
            rootCommand = null;
        }

        [Test, ExpectedArgumentNullException]
        public void TestModelCannotBeNull()
        {
            TestCommandFactory.BuildCommands(null, Mocks.Stub<Filter<ITest>>(), Mocks.Stub<ITestContextManager>());
        }

        [Test, ExpectedArgumentNullException]
        public void FilterCannotBeNull()
        {
            TestCommandFactory.BuildCommands(model, null, Mocks.Stub<ITestContextManager>());
        }

        [Test, ExpectedArgumentNullException]
        public void ContextManagerCannotBeNull()
        {
            TestCommandFactory.BuildCommands(model, Mocks.Stub<Filter<ITest>>(), null);
        }

        [Test]
        public void RootCommandIsNullIfNoTestsSelected()
        {
            PopulateModelWithTests();

            BuildCommands(new NoneFilter<ITest>());
            Assert.IsNull(rootCommand);
        }

        [Test]
        public void RootCommandIncludesEntireHierarchyIfAllTestsSelected()
        {
            PopulateModelWithTests();

            BuildCommands(new AnyFilter<ITest>());
            AssertCommandStructure("Root", "A", "A1", "A2", "A3", "B", "B1");
            AssertCommandExplicit("Root");
        }

        [Test]
        public void RootCommandIncludesSelectedBranchAndAllDescendantsForInteriorNodes()
        {
            PopulateModelWithTests();

            BuildCommands(new NameFilter<ITest>(new EqualityFilter<string>("A")));
            AssertCommandStructure("Root", "A", "A1", "A2", "A3");
            AssertCommandExplicit("A");
        }

        [Test]
        public void RootCommandIncludesSelectedBranchForLeaves()
        {
            PopulateModelWithTests();

            BuildCommands(new NameFilter<ITest>(new EqualityFilter<string>("A2")));
            AssertCommandStructure("Root", "A", "A2");
            AssertCommandExplicit("A2");
        }

        [Test]
        public void RootCommandIncludesAllSelectedBranchesIfThereAreMultipleSelectedNodesAndOnlyExplicitlySelectsTheFirstMatchingNode()
        {
            PopulateModelWithTests();

            BuildCommands(new NameFilter<ITest>(new RegexFilter(new Regex("A2|B"))));
            AssertCommandStructure("Root", "A", "A2", "B", "B1");
            AssertCommandExplicit("A2", "B");
        }

        [Test, ExpectedException(typeof(ModelException))]
        public void DisallowsSelfDependencies()
        {
            PopulateModelWithTests();
            GetTest("A").AddDependency(GetTest("A"));

            BuildCommands(new AnyFilter<ITest>());
        }

        [Test, ExpectedException(typeof(ModelException))]
        public void DisallowsDependenciesOnAncestors()
        {
            PopulateModelWithTests();
            GetTest("A1").AddDependency(GetTest("Root"));

            BuildCommands(new AnyFilter<ITest>());
        }

        [Test, ExpectedException(typeof(ModelException))]
        public void DisallowsDependenciesOnDescendants()
        {
            PopulateModelWithTests();
            GetTest("Root").AddDependency(GetTest("A2"));

            BuildCommands(new AnyFilter<ITest>());
        }

        [Test, ExpectedException(typeof(ModelException))]
        public void DisallowsCircularDependenciesAmongSiblings()
        {
            PopulateModelWithTests();
            GetTest("A").AddDependency(GetTest("B"));
            GetTest("B").AddDependency(GetTest("A"));

            BuildCommands(new AnyFilter<ITest>());
        }

        [Test, ExpectedException(typeof(ModelException))]
        public void DisallowsCircularDependenciesAmongCousins()
        {
            PopulateModelWithTests();
            GetTest("A1").AddDependency(GetTest("B1"));
            GetTest("B1").AddDependency(GetTest("A1"));

            BuildCommands(new AnyFilter<ITest>());
        }

        [Test, ExpectedException(typeof(ModelException))]
        public void DisallowsCircularDependenciesDueToImplicitOrderingOfBothCommonAncestorsInTheHierarchy()
        {
            PopulateModelWithTests();
            GetTest("A1").AddDependency(GetTest("B"));
            GetTest("B1").AddDependency(GetTest("A"));

            BuildCommands(new AnyFilter<ITest>());
        }

        [Test, ExpectedException(typeof(ModelException))]
        public void DisallowsCircularDependenciesDueToImplicitOrderingOfSingleCommonAncestorInTheHierarchy()
        {
            PopulateModelWithTests();
            GetTest("A1").AddDependency(GetTest("B"));
            GetTest("B1").AddDependency(GetTest("A1"));

            BuildCommands(new AnyFilter<ITest>());
        }

        [Test, ExpectedException(typeof(ModelException))]
        public void DisallowsCircularDependenciesWith3Participants()
        {
            PopulateModelWithTests();
            GetTest("A1").AddDependency(GetTest("A2"));
            GetTest("A2").AddDependency(GetTest("A3"));
            GetTest("A3").AddDependency(GetTest("A1"));

            BuildCommands(new AnyFilter<ITest>());
        }

        [Test]
        public void DependencyOnSiblingCausesSiblingToBeReordered()
        {
            PopulateModelWithTests();
            GetTest("A").AddDependency(GetTest("B"));

            BuildCommands(new AnyFilter<ITest>());
            AssertCommandStructure("Root", "B", "B1", "A", "A1", "A2", "A3");
            AssertCommandExplicit("Root");
            AssertCommandDependency("A", "B");
        }

        [Test]
        public void DependencyAmongSiblingsCausesSiblingsToBeReordered()
        {
            PopulateModelWithTests();
            GetTest("A2").AddDependency(GetTest("A1"));
            GetTest("A2").AddDependency(GetTest("A3"));
            GetTest("A3").AddDependency(GetTest("A1"));

            BuildCommands(new AnyFilter<ITest>());
            AssertCommandStructure("Root", "A", "A1", "A3", "A2", "B", "B1");
            AssertCommandExplicit("Root");
            AssertCommandDependency("A2", "A1");
            AssertCommandDependency("A2", "A3");
            AssertCommandDependency("A3", "A1");
        }

        [Test]
        public void DependenciesAmongCousinsCauseParentsToBeReordered()
        {
            PopulateModelWithTests();
            GetTest("A1").AddDependency(GetTest("A2"));
            GetTest("A2").AddDependency(GetTest("B1"));
            GetTest("A3").AddDependency(GetTest("A1"));

            BuildCommands(new AnyFilter<ITest>());
            AssertCommandStructure("Root", "B", "B1", "A", "A2", "A1", "A3");
            AssertCommandExplicit("Root");
            AssertCommandDependency("A1", "A2");
            AssertCommandDependency("A2", "B1");
            AssertCommandDependency("A3", "A1");
        }

        [Test]
        public void DependencyOnNephewCausesSiblingsToBeReordered()
        {
            PopulateModelWithTests();
            GetTest("A").AddDependency(GetTest("B1"));

            BuildCommands(new AnyFilter<ITest>());
            AssertCommandStructure("Root", "B", "B1", "A", "A1", "A2", "A3");
            AssertCommandExplicit("Root");
            AssertCommandDependency("A", "B1");
        }

        [Test]
        public void DependencyOnAuntCausesParentsToBeReordered()
        {
            PopulateModelWithTests();
            GetTest("A2").AddDependency(GetTest("B"));

            BuildCommands(new AnyFilter<ITest>());
            AssertCommandStructure("Root", "B", "B1", "A", "A1", "A2", "A3");
            AssertCommandExplicit("Root");
            AssertCommandDependency("A2", "B");
        }

        private void BuildCommands(Filter<ITest> filter)
        {
            rootCommand = TestCommandFactory.BuildCommands(model, filter, Mocks.Stub<ITestContextManager>());
        }

        private void PopulateModelWithTests()
        {
            // The tree looks like:
            // Root
            //   +- A
            //   |  + A1
            //   |  + A2
            //   |  + A3
            //   +- B
            //   |  + B1

            model.RootTest.Name = "Root";

            model.RootTest.AddChild(CreateTest("A"));
            model.RootTest.Children[0].AddChild(CreateTest("A1"));
            model.RootTest.Children[0].AddChild(CreateTest("A2"));
            model.RootTest.Children[0].AddChild(CreateTest("A3"));

            model.RootTest.AddChild(CreateTest("B"));
            model.RootTest.Children[1].AddChild(CreateTest("B1"));
        }

        private ITest GetTest(string name)
        {
            foreach (ITest test in TreeUtils.GetPreOrderTraversal<ITest>(model.RootTest, delegate(ITest parent) { return parent.Children; }))
            {
                if (test.Name == name)
                    return test;
            }

            Assert.Fail("Did not find test named '{0}'.", name);
            return null;
        }

        private void AssertCommandStructure(params string[] names)
        {
            IList<ITestCommand> all = rootCommand.GetAllCommands();

            InterimAssert.WithPairs(all, names, delegate(ITestCommand node, string name)
            {
                Assert.AreEqual(name, node.Test.Name);
            });
        }

        private void AssertCommandExplicit(params string[] names)
        {
            foreach (ITestCommand node in rootCommand.PreOrderTraversal)
                Assert.AreEqual(Array.IndexOf(names, node.Test.Name) != -1, node.IsExplicit, "Test '{0}' had incorrect explicit flag.", node.Test.Name);
        }

        private ITestCommand GetCommand(string name)
        {
            foreach (ITestCommand node in rootCommand.PreOrderTraversal)
            {
                if (node.Test.Name == name)
                    return node;
            }

            Assert.Fail("Did not find test named '{0}'.", name);
            return null;
        }

        private void AssertCommandDependency(string fromName, string toName)
        {
            Assert.IsTrue(GetCommand(fromName).Dependencies.Contains(GetCommand(toName)),
                "Missing dependency from '{0}' to '{1}'.", fromName, toName);
        }

        private static ITest CreateTest(string name)
        {
            return new BaseTest(name, null);
        }
    }
}
