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
using Gallio.Common.Collections;
using Gallio.Model.Contexts;
using Gallio.Model.Filters;
using Gallio.Model.Tree;

namespace Gallio.Model.Commands
{
    /// <summary>
    /// Generates test commands from a tree of tests using topological sort to
    /// order tests by dependencies and a test order strategy for tie-breaking.
    /// </summary>
    public class DefaultTestCommandFactory : ITestCommandFactory
    {
        private IComparer<Test> testOrderStrategy;

        /// <summary>
        /// Creates a test command factory with a default test order strategy.
        /// </summary>
        public DefaultTestCommandFactory()
        {
            testOrderStrategy = new DefaultTestOrderStrategy();
        }

        /// <summary>
        /// Gets or sets the test order strategy to use when deciding how test commands
        /// should be sorted.
        /// </summary>
        /// <value>The test order strategy.  Defaults to <see cref="DefaultTestOrderStrategy"/>.</value>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null.</exception>
        public IComparer<Test> TestOrderStrategy
        {
            get { return testOrderStrategy; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                testOrderStrategy = value;
            }
        }

        /// <inheritdoc />
        public ITestCommand BuildCommands(TestModel testModel, FilterSet<ITestDescriptor> filterSet, bool exactFilter, ITestContextManager contextManager)
        {
            if (testModel == null)
                throw new ArgumentNullException("testModel");
            if (filterSet == null)
                throw new ArgumentNullException("filterSet");
            if (contextManager == null)
                throw new ArgumentNullException("contextManager");

            var commands = new Dictionary<Test, ManagedTestCommand>();
            bool hasExplicitAncestor = ! filterSet.HasInclusionRules;
            ManagedTestCommand rootCommand = CreateFilteredClosure(commands, testModel.RootTest, filterSet, exactFilter, 
                hasExplicitAncestor, contextManager);
            if (rootCommand == null)
                return null;

            var siblingDependencies = new MultiMap<ManagedTestCommand, ManagedTestCommand>();
            PopulateCommandDependencies(commands, siblingDependencies);

            SortChildren(rootCommand, siblingDependencies);
            return rootCommand;
        }

        private ManagedTestCommand CreateFilteredClosure(Dictionary<Test, ManagedTestCommand> commands,
            Test test, FilterSet<ITestDescriptor> filterSet, bool exactFilter, bool hasExplicitAncestor, ITestContextManager contextManager)
        {
            FilterSetResult filterSetResult = filterSet.Evaluate(test);

            if (filterSetResult == FilterSetResult.Exclude)
                return null;

            bool isMatch = filterSetResult == FilterSetResult.Include;
            bool isExplicit = isMatch && ! hasExplicitAncestor;
            bool hasExplicitChild = false;

            var children = new List<ManagedTestCommand>(test.Children.Count);
            foreach (Test child in test.Children)
            {
                ManagedTestCommand childMonitor = CreateFilteredClosure(commands, child, filterSet, exactFilter,
                    hasExplicitAncestor || isExplicit, contextManager);
                if (childMonitor != null)
                {
                    children.Add(childMonitor);

                    if (childMonitor.IsExplicit)
                        hasExplicitChild = true;
                }
            }

            if (isMatch || children.Count != 0 || (! exactFilter && hasExplicitAncestor))
                return CreateCommand(commands, test, children, isExplicit || hasExplicitChild, contextManager);

            return null;
        }

        private ManagedTestCommand CreateCommand(Dictionary<Test, ManagedTestCommand> commands,
            Test test, IEnumerable<ManagedTestCommand> children, bool isExplicit, ITestContextManager contextManager)
        {
            var testMonitor = new ManagedTestCommand(contextManager, test, isExplicit);
            foreach (ManagedTestCommand child in children)
                testMonitor.AddChild(child);

            commands.Add(test, testMonitor);
            return testMonitor;
        }

        private void PopulateCommandDependencies(Dictionary<Test, ManagedTestCommand> commands,
            MultiMap<ManagedTestCommand, ManagedTestCommand> siblingDependencies)
        {
            foreach (KeyValuePair<Test, ManagedTestCommand> sourceCommand in commands)
            {
                Test source = sourceCommand.Key;
                foreach (Test target in source.Dependencies)
                {
                    ManagedTestCommand targetCommand;
                    if (commands.TryGetValue(target, out targetCommand))
                    {
                        sourceCommand.Value.AddDependency(targetCommand);

                        SetCommandDependency(commands, siblingDependencies, source, target);
                    }
                }
            }
        }

        private void SetCommandDependency(Dictionary<Test, ManagedTestCommand> commands,
            MultiMap<ManagedTestCommand, ManagedTestCommand> siblingDependencies,
            Test source, Test target)
        {
            if (source == target)
                throw new ModelException(String.Format("Test '{0}' has an invalid dependency on itself.", source.FullName));

            Stack<Test> sourceAncestors = CreateAncestorStack(source);
            Stack<Test> targetAncestors = CreateAncestorStack(target);

            Test sourceAncestor, targetAncestor;
            do
            {
                if (sourceAncestors.Count == 0)
                    throw new ModelException(String.Format("Test '{0}' has an invalid dependency on its own descendant '{1}'.",
                        source.FullName, target.FullName));
                if (targetAncestors.Count == 0)
                    throw new ModelException(String.Format("Test '{0}' has an invalid dependency on its own ancestor '{1}'.",
                        source.FullName, target.FullName));

                sourceAncestor = sourceAncestors.Pop();
                targetAncestor = targetAncestors.Pop();
            }
            while (sourceAncestor == targetAncestor);

            // In order to ensure that the dependency is evaluated in the right order,
            // the current sourceAncestor must be executed after the current targetAncestor.
            // So we create an edge from the sourceAncestor command to its sibling
            // targetAncestor command upon which it depends.
            siblingDependencies.Add(commands[sourceAncestor], commands[targetAncestor]);
        }

        private Stack<Test> CreateAncestorStack(Test test)
        {
            var ancestors = new Stack<Test>();
            do
            {
                ancestors.Push(test);
                test = test.Parent;
            }
            while (test != null);

            return ancestors;
        }

        private void SortChildren(ManagedTestCommand parent, MultiMap<ManagedTestCommand, ManagedTestCommand> siblingDependencies)
        {
            ManagedTestCommand[] children = parent.ChildrenToArray();
            if (children.Length == 0)
                return;

            // Clear the array of children since we are about to reshuffle them.
            parent.ClearChildren();

            // Sort the children by order.  Because the topological sort emits vertices precisely
            // in the ordert that it visits them (depth-first) it will preserve the relative ordering
            // of independent vertices.  So we influence test execution order by pre-sorting.
            // Dependencies will of course interfere with the ordering slightly.  However, if the
            // user explicitly specifies orderings in dependency order then they'll indeed run in
            // that specified order.  -- Jeff.
            SortCommandsByOrder(children);

            // Perform a topological sort of the children using depth-first search.
            // Because at this stage a command only has dependencies on its siblings the depth-first search
            // actually proceeds down the chain of sibling dependencies only; it does not
            // traverse the whole test hierarchy.  -- Jeff.
            Dictionary<ManagedTestCommand, bool> visitedSet = new Dictionary<ManagedTestCommand, bool>();
            Stack<DepthFirstEntry> stack = new Stack<DepthFirstEntry>();

            stack.Push(new DepthFirstEntry(null, children));
            for (;;)
            {
                DepthFirstEntry top = stack.Peek();
                if (top.DependencyEnumerator.MoveNext())
                {
                    ManagedTestCommand current = top.DependencyEnumerator.Current;

                    bool inProgressFlag;
                    if (visitedSet.TryGetValue(current, out inProgressFlag))
                    {
                        if (inProgressFlag)
                            throw new ModelException(String.Format("Found a test dependency cycle involving test '{0}'.",
                                current.Test.FullName));
                    }
                    else
                    {
                        IList<ManagedTestCommand> unorderedDependencies = siblingDependencies[current];
                        if (unorderedDependencies.Count != 0)
                        {
                            visitedSet[current] = true;

                            // We need to sort all visited children so that dependencies run in relative order.
                            ManagedTestCommand[] dependencies = GenericCollectionUtils.ToArray(unorderedDependencies);
                            SortCommandsByOrder(dependencies);

                            stack.Push(new DepthFirstEntry(current, dependencies));
                        }
                        else
                        {
                            parent.AddChild(current);
                            visitedSet[current] = false;
                        }
                    }
                }
                else
                {
                    ManagedTestCommand current = top.Source;
                    if (current == null)
                        break;

                    parent.AddChild(current);
                    visitedSet[current] = false;
                    stack.Pop();
                }
            }

            // Recursively sort the children of this command.
            foreach (ManagedTestCommand child in children)
                SortChildren(child, siblingDependencies);
        }

        private void SortCommandsByOrder(ManagedTestCommand[] commands)
        {
            Array.Sort(commands, (a, b) => testOrderStrategy.Compare(a.Test, b.Test));
        }

        private struct DepthFirstEntry
        {
            public readonly ManagedTestCommand Source;
            public readonly IEnumerator<ManagedTestCommand> DependencyEnumerator;

            public DepthFirstEntry(ManagedTestCommand source, IEnumerable<ManagedTestCommand> dependencies)
            {
                Source = source;
                DependencyEnumerator = dependencies.GetEnumerator();
            }
        }
    }
}