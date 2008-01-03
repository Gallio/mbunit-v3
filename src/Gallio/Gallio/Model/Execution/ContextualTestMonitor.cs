// Copyright 2007 MbUnit Project - http://www.mbunit.com/
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
using Gallio.Contexts;

namespace Gallio.Model.Execution
{
    /// <summary>
    /// An implementation of <see cref="ITestMonitor" /> that notifies a
    /// <see cref="IContextHandler" /> as its state changes.
    /// </summary>
    public class ContextualTestMonitor : ITestMonitor
    {
        private readonly IContextHandler handler;
        private readonly ITest test;
        private readonly bool isExplicit;
        private readonly List<ITestMonitor> children;

        /// <summary>
        /// Creates a test monitor.
        /// </summary>
        /// <param name="handler">The context provider</param>
        /// <param name="test">The test</param>
        /// <param name="isExplicit">True if the test is being executed explicitly</param>
        public ContextualTestMonitor(IContextHandler handler, ITest test, bool isExplicit)
        {
            if (handler == null)
                throw new ArgumentNullException("handler");
            if (test == null)
                throw new ArgumentNullException("test");

            this.handler = handler;
            this.test = test;
            this.isExplicit = isExplicit;

            children = new List<ITestMonitor>();
        }

        /// <inheritdoc />
        public ITest Test
        {
            get { return test; }
        }

        /// <inheritdoc />
        public bool IsExplicit
        {
            get { return isExplicit; }
        }

        /// <inheritdoc />
        public int TestCount
        {
            get
            {
                int count = 0;
                foreach (ITestMonitor monitor in PreOrderTraversal)
                    count += 1;
                return count;
            }
        }

        /// <inheritdoc />
        public IEnumerable<ITestMonitor> Children
        {
            get { return children; }
        }

        /// <inheritdoc />
        public IEnumerable<ITestMonitor> PreOrderTraversal
        {
            get
            {
                return TreeUtils.GetPreOrderTraversal<ITestMonitor>(this, GetChildren);
            }
        }

        private static IEnumerable<ITestMonitor> GetChildren(ITestMonitor node)
        {
            return node.Children;
        }

        /// <inheritdoc />
        public IList<ITestMonitor> GetAllMonitors()
        {
            return new List<ITestMonitor>(PreOrderTraversal);
        }

        /// <inheritdoc />
        public ITestStepMonitor StartTestInstance(ITestStep rootStep)
        {
            if (rootStep == null)
                throw new ArgumentNullException("rootStep");
            if (rootStep.Parent != null || rootStep.TestInstance.Test != test)
                throw new ArgumentException("Expected the root step of an instance of this test.", "rootStep");

            ContextualTestStepMonitor stepMonitor = new ContextualTestStepMonitor(handler, rootStep);
            stepMonitor.Start();
            return stepMonitor;
        }

        /// <inheritdoc />
        public ITestStepMonitor StartTestInstance()
        {
            Context context = Context.CurrentContext;
            return StartTestInstance(new BaseTestStep(new BaseTestInstance(test,
                context != null ? context.TestInstance : null)));
        }

        /// <summary>
        /// Adds a child test monitor.
        /// </summary>
        /// <param name="child">The child to add</param>
        public void AddChild(ContextualTestMonitor child)
        {
            if (child == null)
                throw new ArgumentNullException("child");

            children.Add(child);
        }
    }
}
