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
using System.IO;
using Gallio.Common.Collections;
using Gallio.Common.Reflection;
using Gallio.Runtime.Extensibility;
using Gallio.Runtime.Logging;

namespace Gallio.Model
{
    /// <summary>
    /// Provides services based on the installed set of test frameworks.
    /// </summary>
    public interface ITestFrameworkManager
    {
        /// <summary>
        /// Gets handles for all registered test frameworks.
        /// </summary>
        IList<ComponentHandle<ITestFramework, TestFrameworkTraits>> TestFrameworkHandles { get; }

        /// <summary>
        /// Gets the handle of the fallback test framework used when no other
        /// framework supports a given test file.
        /// </summary>
        ComponentHandle<ITestFramework, TestFrameworkTraits> FallbackTestFrameworkHandle { get; }

        /// <summary>
        /// Gets an aggregate test driver for selected frameworks.
        /// </summary>
        /// <param name="selector">The test framework selector.</param>
        /// <param name="logger">The logger for the test driver.</param>
        /// <returns>The test driver.</returns>
        /// <exception cref="ArgumentNullException">Thrown if 
        /// <paramref name="selector"/> or <paramref name="logger"/> is null.</exception>
        ITestDriver GetTestDriver(TestFrameworkSelector selector, ILogger logger);

        /// <summary>
        /// Selects the test frameworks to use for a collection of files.
        /// </summary>
        /// <param name="selector">The test framework selector.</param>
        /// <param name="files">The test files.</param>
        /// <returns>The map of test framework selections to their associated test files.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="selector"/>
        /// or <paramref name="files"/> is null or contains null.</exception>
        IMultiMap<TestFrameworkSelection, FileInfo> SelectTestFrameworksForFiles(TestFrameworkSelector selector, ICollection<FileInfo> files);

        /// <summary>
        /// Selects the test frameworks to use for a collection of code elements.
        /// </summary>
        /// <param name="selector">The test framework selector.</param>
        /// <param name="codeElements">The code elements.</param>
        /// <returns>The map of test framework selections to their associated code elements.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="selector"/>
        /// or <paramref name="codeElements"/> is null or contains null.</exception>
        IMultiMap<TestFrameworkSelection, ICodeElementInfo> SelectTestFrameworksForCodeElements(TestFrameworkSelector selector, ICollection<ICodeElementInfo> codeElements);
    }
}
