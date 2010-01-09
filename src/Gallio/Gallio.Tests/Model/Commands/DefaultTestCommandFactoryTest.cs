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
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Gallio.Common.Collections;
using Gallio.Framework.Pattern;
using Gallio.Model.Commands;
using Gallio.Model.Contexts;
using Gallio.Model.Schema;
using Gallio.Model.Tree;
using Gallio.Runtime.Loader;
using Gallio.Runtime;
using Gallio.Model;
using Gallio.Model.Filters;
using Gallio.Common.Reflection;
using MbUnit.Framework;
using Test=Gallio.Model.Tree.Test;

namespace Gallio.Tests.Model.Commands
{
    [TestFixture]
    [TestsOn(typeof(DefaultTestCommandFactory))]
    public class DefaultTestCommandFactoryTest : BaseTestWithMocks
    {
        private TestModel model;
        private ITestCommand rootCommand;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            model = new TestModel();
            rootCommand = null;
        }

        [Test, ExpectedArgumentNullException]
        public void TestModelCannotBeNull()
        {
            var testCommandFactory = new DefaultTestCommandFactory();

            testCommandFactory.BuildCommands(null, FilterSet<ITestDescriptor>.Empty, false, Mocks.Stub<ITestContextManager>());
        }

        [Test, ExpectedArgumentNullException]
        public void FilterCannotBeNull()
        {
            var testCommandFactory = new DefaultTestCommandFactory();

            testCommandFactory.BuildCommands(model, null, false, Mocks.Stub<ITestContextManager>());
        }

        [Test, ExpectedArgumentNullException]
        public void ContextManagerCannotBeNull()
        {
            var testCommandFactory = new DefaultTestCommandFactory();

            testCommandFactory.BuildCommands(model, FilterSet<ITestDescriptor>.Empty, false, null);
        }

        [Test]
        public void RootCommandIsNullIfNoTestsSelected()
        {
            PopulateModelWithTests();

            BuildCommands(new NoneFilter<ITestDescriptor>(), false);
            Assert.IsNull(rootCommand);
        }

        [Test]
        public void RootCommandIncludesEntireHierarchyIfFilterSetSelectsAllTestsSelected()
        {
            PopulateModelWithTests();

            BuildCommands(new AnyFilter<ITestDescriptor>(), false);
            AssertCommandStructure("Root", "A", "A1", "A2", "A3", "B", "B1");
            AssertCommandExplicit("Root");
        }

        [Test]
        public void RootCommandIncludesEntireHierarchyIfFilterSetIsEmpty()
        {
            PopulateModelWithTests();

            BuildCommands(FilterSet<ITestDescriptor>.Empty, false);
            AssertCommandStructure("Root", "A", "A1", "A2", "A3", "B", "B1");
            AssertCommandExplicit();
        }

        [Test]
        public void RootCommandIncludesSelectedBranchAndAllDescendantsForInteriorNodes()
        {
            PopulateModelWithTests();

            BuildCommands(new NameFilter<ITestDescriptor>(new EqualityFilter<string>("A")), false);
            AssertCommandStructure("Root", "A", "A1", "A2", "A3");
            AssertCommandExplicit("Root", "A");
        }

        [Test]
        public void RootCommandIncludesOnlyNonExcludedTestsInHierarchyEvenIfAChildTestMightHaveBeenOtherwiseIncluded()
        {
            PopulateModelWithTests();

            BuildCommands(new FilterSet<ITestDescriptor>(new[] { new FilterRule<ITestDescriptor>(FilterRuleType.Exclusion, new NameFilter<ITestDescriptor>(new EqualityFilter<string>("A"))) }), false);
            AssertCommandStructure("Root", "B", "B1");
            AssertCommandExplicit();
        }

        [Test]
        public void RootCommandIncludesExactlySpecifiedNodesWhenFilterIsExact()
        {
            PopulateModelWithTests();

            BuildCommands(new NameFilter<ITestDescriptor>(new EqualityFilter<string>("A")), true);
            AssertCommandStructure("Root", "A");
            AssertCommandExplicit("Root", "A");
        }

        [Test]
        public void RootCommandIncludesSelectedBranchForLeaves()
        {
            PopulateModelWithTests();

            BuildCommands(new NameFilter<ITestDescriptor>(new EqualityFilter<string>("A2")), false);
            AssertCommandStructure("Root", "A", "A2");
            AssertCommandExplicit("Root", "A", "A2");
        }

        [Test]
        public void RootCommandIncludesAllSelectedBranchesIfThereAreMultipleSelectedNodesAndOnlyExplicitlySelectsTheFirstMatchingNode()
        {
            PopulateModelWithTests();

            BuildCommands(new NameFilter<ITestDescriptor>(new RegexFilter(new Regex("A2|B"))), false);
            AssertCommandStructure("Root", "A", "A2", "B", "B1");
            AssertCommandExplicit("Root", "A", "A2", "B");
        }

        [Test, ExpectedException(typeof(ModelException))]
        public void DisallowsSelfDependencies()
        {
            PopulateModelWithTests();
            GetTest("A").AddDependency(GetTest("A"));

            BuildCommands(new AnyFilter<ITestDescriptor>(), false);
        }

        [Test, ExpectedException(typeof(ModelException))]
        public void DisallowsDependenciesOnAncestors()
        {
            PopulateModelWithTests();
            GetTest("A1").AddDependency(GetTest("Root"));

            BuildCommands(new AnyFilter<ITestDescriptor>(), false);
        }

        [Test, ExpectedException(typeof(ModelException))]
        public void DisallowsDependenciesOnDescendants()
        {
            PopulateModelWithTests();
            GetTest("Root").AddDependency(GetTest("A2"));

            BuildCommands(new AnyFilter<ITestDescriptor>(), false);
        }

        [Test, ExpectedException(typeof(ModelException))]
        public void DisallowsCircularDependenciesAmongSiblings()
        {
            PopulateModelWithTests();
            GetTest("A").AddDependency(GetTest("B"));
            GetTest("B").AddDependency(GetTest("A"));

            BuildCommands(new AnyFilter<ITestDescriptor>(), false);
        }

        [Test, ExpectedException(typeof(ModelException))]
        public void DisallowsCircularDependenciesAmongCousins()
        {
            PopulateModelWithTests();
            GetTest("A1").AddDependency(GetTest("B1"));
            GetTest("B1").AddDependency(GetTest("A1"));

            BuildCommands(new AnyFilter<ITestDescriptor>(), false);
        }

        [Test, ExpectedException(typeof(ModelException))]
        public void DisallowsCircularDependenciesDueToImplicitOrderingOfBothCommonAncestorsInTheHierarchy()
        {
            PopulateModelWithTests();
            GetTest("A1").AddDependency(GetTest("B"));
            GetTest("B1").AddDependency(GetTest("A"));

            BuildCommands(new AnyFilter<ITestDescriptor>(), false);
        }

        [Test, ExpectedException(typeof(ModelException))]
        public void DisallowsCircularDependenciesDueToImplicitOrderingOfSingleCommonAncestorInTheHierarchy()
        {
            PopulateModelWithTests();
            GetTest("A1").AddDependency(GetTest("B"));
            GetTest("B1").AddDependency(GetTest("A1"));

            BuildCommands(new AnyFilter<ITestDescriptor>(), false);
        }

        [Test, ExpectedException(typeof(ModelException))]
        public void DisallowsCircularDependenciesWith3Participants()
        {
            PopulateModelWithTests();
            GetTest("A1").AddDependency(GetTest("A2"));
            GetTest("A2").AddDependency(GetTest("A3"));
            GetTest("A3").AddDependency(GetTest("A1"));

            BuildCommands(new AnyFilter<ITestDescriptor>(), false);
        }

        [Test]
        public void DependencyOnSiblingCausesSiblingToBeReordered()
        {
            PopulateModelWithTests();
            GetTest("A").AddDependency(GetTest("B"));

            BuildCommands(new AnyFilter<ITestDescriptor>(), false);
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

            BuildCommands(new AnyFilter<ITestDescriptor>(), false);
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

            BuildCommands(new AnyFilter<ITestDescriptor>(), false);
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

            BuildCommands(new AnyFilter<ITestDescriptor>(), false);
            AssertCommandStructure("Root", "B", "B1", "A", "A1", "A2", "A3");
            AssertCommandExplicit("Root");
            AssertCommandDependency("A", "B1");
        }

        [Test]
        public void DependencyOnAuntCausesParentsToBeReordered()
        {
            PopulateModelWithTests();
            GetTest("A2").AddDependency(GetTest("B"));

            BuildCommands(new AnyFilter<ITestDescriptor>(), false);
            AssertCommandStructure("Root", "B", "B1", "A", "A1", "A2", "A3");
            AssertCommandExplicit("Root");
            AssertCommandDependency("A2", "B");
        }

        [Test]
        public void IndependentTestsAreSortedByOrder()
        {
            model.RootTest.Name = "Root";

            model.RootTest.AddChild(CreateTest("A", 1));
            model.RootTest.Children[0].AddChild(CreateTest("A1", 2));
            model.RootTest.Children[0].AddChild(CreateTest("A2", 0));
            model.RootTest.Children[0].AddChild(CreateTest("A3", 1));

            model.RootTest.AddChild(CreateTest("B", 0));
            model.RootTest.Children[1].AddChild(CreateTest("B1", 0));

            BuildCommands(new AnyFilter<ITestDescriptor>(), false);
            AssertCommandStructure("Root", "B", "B1", "A", "A2", "A3", "A1");
            AssertCommandExplicit("Root");
        }

        [Test]
        public void IndependentTestsOfSameOrderAreSortedByName()
        {
            model.RootTest.Name = "Root";

            model.RootTest.AddChild(CreateTest("B", 0));
            model.RootTest.Children[0].AddChild(CreateTest("B1", 0));

            model.RootTest.AddChild(CreateTest("A", 0));
            model.RootTest.Children[1].AddChild(CreateTest("A3", 0));
            model.RootTest.Children[1].AddChild(CreateTest("A1", 0));
            model.RootTest.Children[1].AddChild(CreateTest("A2", 0));

            BuildCommands(new AnyFilter<ITestDescriptor>(), false);
            AssertCommandStructure("Root", "A", "A1", "A2", "A3", "B", "B1");
            AssertCommandExplicit("Root");
        }

        [Test]
        public void DependentTestsAreSortedByOrder()
        {
            model.RootTest.Name = "Root";

            model.RootTest.AddChild(CreateTest("A", 1));
            model.RootTest.Children[0].AddChild(CreateTest("A1", 2));
            model.RootTest.Children[0].AddChild(CreateTest("A2", 0));
            model.RootTest.Children[0].AddChild(CreateTest("A3", 1));

            model.RootTest.AddChild(CreateTest("B", 0));
            model.RootTest.Children[1].AddChild(CreateTest("B1", 0));

            GetTest("A2").AddDependency(GetTest("A1"));
            GetTest("A2").AddDependency(GetTest("A3"));

            BuildCommands(new AnyFilter<ITestDescriptor>(), false);
            AssertCommandStructure("Root", "B", "B1", "A", "A3", "A1", "A2");
            AssertCommandExplicit("Root");
        }

        private void BuildCommands(Filter<ITestDescriptor> filter, bool exactFilter)
        {
            BuildCommands(new FilterSet<ITestDescriptor>(filter), exactFilter);
        }

        private void BuildCommands(FilterSet<ITestDescriptor> filterSet, bool exactFilter)
        {
            var testCommandFactory = new DefaultTestCommandFactory();

            rootCommand = testCommandFactory.BuildCommands(model, filterSet, exactFilter, Mocks.Stub<ITestContextManager>());
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

            model.RootTest.AddChild(CreateTest("A", 0));
            model.RootTest.Children[0].AddChild(CreateTest("A1", 0));
            model.RootTest.Children[0].AddChild(CreateTest("A2", 0));
            model.RootTest.Children[0].AddChild(CreateTest("A3", 0));

            model.RootTest.AddChild(CreateTest("B", 0));
            model.RootTest.Children[1].AddChild(CreateTest("B1", 0));
        }

        private Test GetTest(string name)
        {
            foreach (Test test in TreeUtils.GetPreOrderTraversal<Test>(model.RootTest, delegate(Test parent) { return parent.Children; }))
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

            Assert.Over.Pairs(all, names, (node, name) => Assert.AreEqual(name, node.Test.Name));
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

        private static Test CreateTest(string name, int order)
        {
            return new Test(name, null) { Order = order };
        }
    }
}
