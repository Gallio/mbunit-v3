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
using Gallio.Model.Contexts;
using Gallio.Model.Filters;
using Gallio.Model.Tree;

namespace Gallio.Model.Commands
{
    /// <summary>
    /// Generates test commands from a tree of tests.
    /// </summary>
    public interface ITestCommandFactory
    {
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
        ITestCommand BuildCommands(TestModel testModel, FilterSet<ITestDescriptor> filterSet, bool exactFilter, ITestContextManager contextManager);
    }
}
