// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
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
using Gallio.Model.Filters;

namespace Gallio.Model.Execution
{
    /// <summary>
    /// Creates test commands.
    /// </summary>
    public static class TestCommandFactory
    {
        private static readonly DefaultTestOrderStrategy testOrderStrategy = new DefaultTestOrderStrategy();

        /// <summary>
        /// Recursively builds a tree of test commands.
        /// </summary>
        /// <param name="testModel">The test model.</param>
        /// <param name="filterSet">The filter set for the test model.</param>
        /// <param name="exactFilter">If true, only the specified tests are included, otherwise children
        /// of the selected tests are automatically included.</param>
        /// <param name="contextManager">The test context manager.</param>
        /// <returns>The root test command or null if none of the tests in
        /// the subtree including <paramref name="testModel"/> matched the filter.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="testModel"/>,
        /// <paramref name="filterSet"/> or <paramref name="contextManager"/> is null.</exception>
        /// <exception cref="ModelException">Thrown if an invalid test dependency is found.</exception>
        public static ITestCommand BuildCommands(TestModel testModel, FilterSet<ITest> filterSet, bool exactFilter, ITestContextManager contextManager)
        {
            if (testModel == null)
                throw new ArgumentNullException("testModel");
            if (filterSet == null)
                throw new ArgumentNullException("filterSet");
            if (contextManager == null)
                throw new ArgumentNullException("contextManager");

            var commands = new Dictionary<ITest, ManagedTestCommand>();
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

        private static ManagedTestCommand CreateFilteredClosure(Dictionary<ITest, ManagedTestCommand> commands,
            ITest test, FilterSet<ITest> filterSet, bool exactFilter, bool hasExplicitAncestor, ITestContextManager contextManager)
        {
            FilterSetResult filterSetResult = filterSet.Evaluate(test);

            if (filterSetResult == FilterSetResult.Exclude)
                return null;

            bool isMatch = filterSetResult == FilterSetResult.Include;
            bool isExplicit = isMatch && ! hasExplicitAncestor;
            bool hasExplicitChild = false;

            var children = new List<ManagedTestCommand>(test.Children.Count);
            foreach (ITest child in test.Children)
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

        private static ManagedTestCommand CreateCommand(Dictionary<ITest, ManagedTestCommand> commands,
            ITest test, IEnumerable<ManagedTestCommand> children, bool isExplicit, ITestContextManager contextManager)
        {
            var testMonitor = new ManagedTestCommand(contextManager, test, isExplicit);
            foreach (ManagedTestCommand child in children)
                testMonitor.AddChild(child);

            commands.Add(test, testMonitor);
            return testMonitor;
        }

        private static void PopulateCommandDependencies(Dictionary<ITest, ManagedTestCommand> commands,
            MultiMap<ManagedTestCommand, ManagedTestCommand> siblingDependencies)
        {
            foreach (KeyValuePair<ITest, ManagedTestCommand> sourceCommand in commands)
            {
                ITest source = sourceCommand.Key;
                foreach (ITest target in source.Dependencies)
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

        private static void SetCommandDependency(Dictionary<ITest, ManagedTestCommand> commands,
            MultiMap<ManagedTestCommand, ManagedTestCommand> siblingDependencies,
            ITest source, ITest target)
        {
            if (source == target)
                throw new ModelException(String.Format("Test '{0}' has an invalid dependency on itself.", source.FullName));

            Stack<ITest> sourceAncestors = CreateAncestorStack(source);
            Stack<ITest> targetAncestors = CreateAncestorStack(target);

            ITest sourceAncestor, targetAncestor;
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

        private static Stack<ITest> CreateAncestorStack(ITest test)
        {
            var ancestors = new Stack<ITest>();
            do
            {
                ancestors.Push(test);
                test = test.Parent;
            }
            while (test != null);

            return ancestors;
        }

        private static void SortChildren(ManagedTestCommand parent, MultiMap<ManagedTestCommand, ManagedTestCommand> siblingDependencies)
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

        private static void SortCommandsByOrder(ManagedTestCommand[] commands)
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
