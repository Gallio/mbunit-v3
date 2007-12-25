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
using Gallio.Reflection;
using Gallio.Hosting;

namespace Gallio.Model
{
    /// <summary>
    /// An aggregate test explorer combines multiple test explorers from
    /// different sources to incorporate all of their contributions.
    /// </summary>
    public class AggregateTestExplorer : ITestExplorer
    {
        private readonly List<ITestExplorer> explorers;

        /// <summary>
        /// Creates an empty aggregate test explorer.
        /// </summary>
        public AggregateTestExplorer()
        {
            explorers = new List<ITestExplorer>();
        }

        /// <summary>
        /// Creates an aggregate test explorer from explorers created for
        /// all registered <see cref="ITestFramework" /> services in the
        /// <see cref="Runtime" />.
        /// </summary>
        /// <param name="testModel">The test model to populate incrementally as
        /// tests are discovered</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="testModel"/> is null</exception>
        public static AggregateTestExplorer CreateExplorerForAllTestFrameworks(TestModel testModel)
        {
            if (testModel == null)
                throw new ArgumentNullException("testModel");

            AggregateTestExplorer aggregate = new AggregateTestExplorer();

            foreach (ITestFramework framework in Runtime.Instance.ResolveAll<ITestFramework>())
                aggregate.AddTestExplorer(framework.CreateTestExplorer(testModel));

            return aggregate;
        }

        TestModel ITestExplorer.TestModel
        {
            get { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Adds a test explorer to the aggregate.
        /// </summary>
        /// <param name="explorer">The explorer to add</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="explorer"/> is null</exception>
        public void AddTestExplorer(ITestExplorer explorer)
        {
            if (explorer == null)
                throw new ArgumentNullException("explorer");

            explorers.Add(explorer);
        }

        /// <inheritdoc />
        public bool IsTest(ICodeElementInfo element)
        {
            foreach (ITestExplorer explorer in explorers)
                if (explorer.IsTest(element))
                    return true;

            return false;
        }

        /// <inheritdoc />
        public void ExploreAssembly(IAssemblyInfo assembly, Action<ITest> consumer)
        {
            foreach (ITestExplorer explorer in explorers)
                explorer.ExploreAssembly(assembly, consumer);
        }

        /// <inheritdoc />
        public void ExploreType(ITypeInfo type, Action<ITest> consumer)
        {
            foreach (ITestExplorer explorer in explorers)
                explorer.ExploreType(type, consumer);
        }
    }
}
