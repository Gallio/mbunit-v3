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
using System.Text;
using System.Threading;
using Gallio.Common.Collections;
using Gallio.Model.Contexts;
using Gallio.Model.Tree;

namespace Gallio.Model.Commands
{
    /// <summary>
    /// A <see cref="ITestCommand"/> implementation based on a <see cref="ITestContextManager"/>.
    /// </summary>
    public class ManagedTestCommand : ITestCommand
    {
        private readonly ITestContextManager contextManager;
        private readonly Test test;
        private readonly bool isExplicit;

        private List<ITestCommand> children;
        private List<ITestCommand> dependencies;
        private int rootStepFailureCount;

        /// <summary>
        /// Creates a test command.
        /// </summary>
        /// <param name="contextManager">The test context manager.</param>
        /// <param name="test">The test.</param>
        /// <param name="isExplicit">True if the test is being executed explicitly.</param>
        public ManagedTestCommand(ITestContextManager contextManager, Test test, bool isExplicit)
        {
            if (contextManager == null)
                throw new ArgumentNullException("contextManager");
            if (test == null)
                throw new ArgumentNullException("test");

            this.contextManager = contextManager;
            this.test = test;
            this.isExplicit = isExplicit;
        }

        /// <inheritdoc />
        public Test Test
        {
            get { return test; }
        }

        /// <inheritdoc />
        public bool IsExplicit
        {
            get { return isExplicit; }
        }

        /// <inheritdoc />
        public int RootStepFailureCount
        {
            get { return rootStepFailureCount; }
        }

        /// <inheritdoc />
        public int TestCount
        {
            get
            {
                int count = 0;
                foreach (ITestCommand monitor in PreOrderTraversal)
                    count += 1;
                return count;
            }
        }

        /// <inheritdoc />
        public IList<ITestCommand> Children
        {
            get { return children ?? (IList<ITestCommand>)EmptyArray<ITestCommand>.Instance; }
        }

        /// <inheritdoc />
        public IList<ITestCommand> Dependencies
        {
            get { return dependencies ?? (IList<ITestCommand>)EmptyArray<ITestCommand>.Instance; }
        }

        /// <inheritdoc />
        public IEnumerable<ITestCommand> PreOrderTraversal
        {
            get
            {
                return TreeUtils.GetPreOrderTraversal<ITestCommand>(this, GetChildren);
            }
        }

        private static IEnumerable<ITestCommand> GetChildren(ITestCommand node)
        {
            return node.Children;
        }

        /// <inheritdoc />
        public IList<ITestCommand> GetAllCommands()
        {
            return new List<ITestCommand>(PreOrderTraversal);
        }

        /// <inheritdoc />
        public bool AreDependenciesSatisfied()
        {
            if (dependencies != null)
            {
                foreach (ITestCommand dependency in dependencies)
                {
                    if (dependency.RootStepFailureCount != 0
                        || ! dependency.AreDependenciesSatisfied())
                        return false;
                }
            }

            return true;
        }

        /// <inheritdoc />
        public ITestContext StartStep(TestStep testStep)
        {
            if (testStep == null)
                throw new ArgumentNullException("testStep");
            if (testStep.Test != test)
                throw new ArgumentException("The test step must belong to the test associated with this test command.", "testStep");

            ITestContext context = contextManager.StartStep(testStep);
            context.Finishing += UpdateFailureCount;
            return context;
        }

        private void UpdateFailureCount(object sender, EventArgs e)
        {
            ITestContext context = (ITestContext)sender;
            if (context.Outcome.Status == TestStatus.Failed)
                Interlocked.Increment(ref rootStepFailureCount);
        }

        /// <inheritdoc />
        public ITestContext StartPrimaryChildStep(TestStep parentTestStep)
        {
            TestStep primaryStep = new TestStep(test, parentTestStep);
            return StartStep(primaryStep);
        }

        /// <summary>
        /// Adds a child test command.
        /// </summary>
        /// <param name="child">The child to add.</param>
        public void AddChild(ManagedTestCommand child)
        {
            if (child == null)
                throw new ArgumentNullException("child");

            if (children == null)
                children = new List<ITestCommand>();
            children.Add(child);
        }

        /// <summary>
        /// Adds a test command dependency.
        /// </summary>
        /// <param name="dependency">The dependency to add.</param>
        public void AddDependency(ManagedTestCommand dependency)
        {
            if (dependency == null)
                throw new ArgumentNullException("dependency");

            if (dependencies == null)
                dependencies = new List<ITestCommand>();
            dependencies.Add(dependency);
        }

        /// <summary>
        /// Clears the children of the command.
        /// </summary>
        public void ClearChildren()
        {
            if (children != null)
                children.Clear();
        }

        /// <summary>
        /// Gets the list of children as an array.
        /// </summary>
        /// <returns>The array of children.</returns>
        public ManagedTestCommand[] ChildrenToArray()
        {
            if (children == null)
                return EmptyArray<ManagedTestCommand>.Instance;

            ManagedTestCommand[] array = new ManagedTestCommand[children.Count];
            children.CopyTo(array);
            return array;
        }

        /// <summary>
        /// Returns a description of the test command for debugging purposes.
        /// </summary>
        /// <returns>A description of the test command.</returns>
        public override string ToString()
        {
            StringBuilder description = new StringBuilder(test.Name);
            if (isExplicit)
                description.Append(" (explicit)");
            return description.ToString();
        }
    }
}