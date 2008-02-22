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
using Gallio.Collections;
using Gallio.Model.Filters;

namespace Gallio.Model.Execution
{
    /// <summary>
    /// Creates test commands.
    /// </summary>
    public static class TestCommandFactory
    {
        /// <summary>
        /// Recursively builds a tree of test commands.
        /// </summary>
        /// <param name="testModel">The test model</param>
        /// <param name="filter">The filter for the test model</param>
        /// <param name="contextManager">The test context manager</param>
        /// <returns>The root test command or null if none of the tests in
        /// the subtree including <paramref name="testModel"/> matched the filter</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="testModel"/>,
        /// <paramref name="filter"/> or <paramref name="contextManager"/> is null</exception>
        /// <exception cref="ModelException">Thrown if an invalid test dependency is found</exception>
        public static ITestCommand BuildCommands(TestModel testModel, Filter<ITest> filter, ITestContextManager contextManager)
        {
            if (testModel == null)
                throw new ArgumentNullException("testModel");
            if (filter == null)
                throw new ArgumentNullException("filter");
            if (contextManager == null)
                throw new ArgumentNullException("contextManager");

            Dictionary<ITest, ManagedTestCommand> commands = new Dictionary<ITest, ManagedTestCommand>();
            ManagedTestCommand rootCommand = CreateFilteredClosure(commands, testModel.RootTest, filter, contextManager);
            if (rootCommand == null)
                return null;

            MultiMap<ManagedTestCommand, ManagedTestCommand> orderedSiblings = new MultiMap<ManagedTestCommand, ManagedTestCommand>();
            PopulateCommandDependencies(commands, orderedSiblings);

            SortChildren(rootCommand, orderedSiblings);
            return rootCommand;
        }

        private static ManagedTestCommand CreateFilteredClosure(Dictionary<ITest, ManagedTestCommand> commands,
            ITest test, Filter<ITest> filter, ITestContextManager contextManager)
        {
            if (filter.IsMatch(test))
                return CreateCommandSubtree(commands, test, true, contextManager);

            List<ManagedTestCommand> children = new List<ManagedTestCommand>(test.Children.Count);

            foreach (ITest child in test.Children)
            {
                ManagedTestCommand childMonitor = CreateFilteredClosure(commands, child, filter, contextManager);
                if (childMonitor != null)
                    children.Add(childMonitor);
            }

            if (children.Count != 0)
                return CreateCommand(commands, test, children, false, contextManager);

            return null;
        }

        private static ManagedTestCommand CreateCommandSubtree(Dictionary<ITest, ManagedTestCommand> commands,
            ITest test, bool isExplicit, ITestContextManager contextManager)
        {
            List<ManagedTestCommand> children = new List<ManagedTestCommand>(test.Children.Count);

            foreach (ITest child in test.Children)
                children.Add(CreateCommandSubtree(commands, child, false, contextManager));

            return CreateCommand(commands, test, children, isExplicit, contextManager);
        }

        private static ManagedTestCommand CreateCommand(Dictionary<ITest, ManagedTestCommand> commands,
            ITest test, IEnumerable<ManagedTestCommand> children, bool isExplicit, ITestContextManager contextManager)
        {
            ManagedTestCommand testMonitor = new ManagedTestCommand(contextManager, test, isExplicit);
            foreach (ManagedTestCommand child in children)
                testMonitor.AddChild(child);

            commands.Add(test, testMonitor);
            return testMonitor;
        }

        private static void PopulateCommandDependencies(Dictionary<ITest, ManagedTestCommand> commands,
            MultiMap<ManagedTestCommand, ManagedTestCommand> orderedSiblings)
        {
            foreach (KeyValuePair<ITest, ManagedTestCommand> entry in commands)
            {
                foreach (ITest testDependency in entry.Key.Dependencies)
                {
                    ManagedTestCommand commandDependency;
                    if (commands.TryGetValue(testDependency, out commandDependency))
                    {
                        entry.Value.AddDependency(commandDependency);

                        CreateOrderedEdgeForDependency(commands, orderedSiblings, entry.Key, testDependency);
                    }
                }
            }
        }

        private static void CreateOrderedEdgeForDependency(Dictionary<ITest, ManagedTestCommand> commands,
            MultiMap<ManagedTestCommand, ManagedTestCommand> orderedSiblings,
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
            // the current sourceAncestor must be executed before the current targetAncestor.
            // So we create an edge from the sourceAncestor command to its sibling
            // targetAncestor command.
            orderedSiblings.Add(commands[sourceAncestor], commands[targetAncestor]);
        }

        private static Stack<ITest> CreateAncestorStack(ITest test)
        {
            Stack<ITest> ancestors = new Stack<ITest>();
            do
            {
                ancestors.Push(test);
                test = test.Parent;
            }
            while (test != null);

            return ancestors;
        }

        private static void SortChildren(ManagedTestCommand parent, MultiMap<ManagedTestCommand, ManagedTestCommand> orderedSiblings)
        {
            IList<ManagedTestCommand> children = parent.ChildrenToArray();
            if (children.Count == 0)
                return;

            // Perform a topological sort of the children using depth-first search.
            parent.ClearChildren();

            Dictionary<ManagedTestCommand, bool> visitedSet = new Dictionary<ManagedTestCommand, bool>();
            Stack<DepthFirstEntry> stack = new Stack<DepthFirstEntry>();

            stack.Push(new DepthFirstEntry(null, children.GetEnumerator()));
            for (;;)
            {
                DepthFirstEntry top = stack.Peek();
                if (top.EdgeEnumerator.MoveNext())
                {
                    ManagedTestCommand current = top.EdgeEnumerator.Current;
                    bool inProgressFlag;
                    if (visitedSet.TryGetValue(current, out inProgressFlag))
                    {
                        if (inProgressFlag)
                            throw new ModelException(String.Format("Found a test dependency cycle involving test '{0}'.",
                                current.Test.FullName));
                    }
                    else
                    {
                        visitedSet[current] = true;
                        stack.Push(new DepthFirstEntry(current, orderedSiblings[current].GetEnumerator()));
                    }
                }
                else
                {
                    ManagedTestCommand current = top.Vertex;
                    if (current == null)
                        break;

                    parent.AddChild(current);
                    visitedSet[current] = false;
                    stack.Pop();
                }
            }

            // Recursively sort the children of this command.
            foreach (ManagedTestCommand child in children)
                SortChildren(child, orderedSiblings);
        }

        private struct DepthFirstEntry
        {
            public readonly ManagedTestCommand Vertex;
            public readonly IEnumerator<ManagedTestCommand> EdgeEnumerator;

            public DepthFirstEntry(ManagedTestCommand vertex, IEnumerator<ManagedTestCommand> edgeEnumerator)
            {
                Vertex = vertex;
                EdgeEnumerator = edgeEnumerator;
            }
        }
    }
}
