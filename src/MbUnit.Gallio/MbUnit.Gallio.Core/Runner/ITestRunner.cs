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
using MbUnit.Core.Serialization;
using MbUnit.Framework.Kernel.Harness;
using MbUnit.Framework.Services.Runtime;

namespace MbUnit.Core.Runner
{
    /// <summary>
    /// A test runner provides operations for loading test projects, enumerating
    /// templates and tests, running tests, and generating reports.
    /// </summary>
    /// <remarks>
    /// This interface is primarily used to simplify test runner integration
    /// concerns by gathering the entire lifecycle in one place.
    /// </remarks>
    /// <todo author="jeff">
    /// I don't like the direction this API is taking.  I intended it to capture
    /// state management and lifecycle concerns so that test runners don't need
    /// to bother with some of the minor steps if they don't care about the
    /// intermediate results.  However, this is starting to look like a rather
    /// ugly version of <see cref="ITestDomain" /> with confusing rules as regards
    /// state.  Aggregating <see cref="ITestDomain" /> is a possibility.  Suggestions?
    /// </todo>
    public interface ITestRunner : IDisposable
    {
        /// <summary>
        /// Gets or sets the template enumeration options.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
        TemplateEnumerationOptions TemplateEnumerationOptions { get; set; }

        /// <summary>
        /// Gets or sets the test enumeration options.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
        TestEnumerationOptions TestEnumerationOptions { get; set; }

        /// <summary>
        /// Gets or sets the test execution options.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
        TestExecutionOptions TestExecutionOptions { get; set; }

        /// <summary>
        /// Loads a test project.
        /// </summary>
        /// <param name="project">The test project</param>
        void LoadProject(TestProject project);

        /// <summary>
        /// Gets the root of the template tree.
        /// Automatically builds the template tree if needed.
        /// </summary>
        TemplateInfo GetTemplateTreeRoot();

        /// <summary>
        /// Gets the root of the test tree.
        /// Automatically builds the test tree if needed.
        /// </summary>
        TestInfo GetTestTreeRoot();

        /// <summary>
        /// Runs the tests.
        /// </summary>
        void Run();

        /// <summary>
        /// Writes a test report.
        /// </summary>
        void WriteReport();
    }
}