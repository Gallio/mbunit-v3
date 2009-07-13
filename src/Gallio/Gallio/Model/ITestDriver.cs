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
using Gallio.Common.Reflection;
using Gallio.Common.Messaging;
using Gallio.Model.Isolation;
using Gallio.Runtime.ProgressMonitoring;

namespace Gallio.Model
{
    /// <summary>
    /// Provides test framework services for exploring and running tests.
    /// </summary>
    public interface ITestDriver
    {
        /// <summary>
        /// Returns true if the code element represents a test.
        /// </summary>
        /// <param name="reflectionPolicy">The reflection policy.</param>
        /// <param name="codeElement">The code element.</param>
        /// <returns>True if the code element represents a test.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="reflectionPolicy"/>
        /// or <paramref name="codeElement"/> is null.</exception>
        bool IsTest(IReflectionPolicy reflectionPolicy, ICodeElementInfo codeElement);

        /// <summary>
        /// Returns true if the code element represents a part of a test such as a setup or teardown method.
        /// </summary>
        /// <param name="reflectionPolicy">The reflection policy.</param>
        /// <param name="codeElement">The code element.</param>
        /// <returns>True if the code element represents a part of a test.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="reflectionPolicy"/>
        /// or <paramref name="codeElement"/> is null.</exception>
        bool IsTestPart(IReflectionPolicy reflectionPolicy, ICodeElementInfo codeElement);

        /// <summary>
        /// Describes tests via reflection over code elements.
        /// </summary>
        /// <param name="reflectionPolicy">The reflection policy.</param>
        /// <param name="codeElements">The list of code elements, usually <see cref="ITypeInfo" />
        /// and <see cref="IAssemblyInfo" /> objects, that might contain tests to be described.</param>
        /// <param name="testExplorationOptions">The test exploration options.</param>
        /// <param name="messageSink">The message sink to receive test exploration messages.</param>
        /// <param name="progressMonitor">The progress monitor.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="reflectionPolicy"/>,
        /// <paramref name="codeElements"/>, <paramref name="testExplorationOptions"/>, <paramref name="messageSink"/>
        /// or <paramref name="progressMonitor"/> is null.</exception>
        void Describe(IReflectionPolicy reflectionPolicy, IList<ICodeElementInfo> codeElements,
            TestExplorationOptions testExplorationOptions, IMessageSink messageSink, IProgressMonitor progressMonitor);

        /// <summary>
        /// Explores tests a the test package.
        /// </summary>
        /// <param name="testIsolationContext">The test isolation context.</param>
        /// <param name="testPackage">The test package.</param>
        /// <param name="testExplorationOptions">The test exploration options.</param>
        /// <param name="messageSink">The message sink to receive test exploration messages.</param>
        /// <param name="progressMonitor">The progress monitor.</param>
        /// <returns>The test report.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="testIsolationContext"/>,
        /// <paramref name="testPackage"/>, <paramref name="testExplorationOptions"/>, <paramref name="messageSink"/>
        /// or <paramref name="progressMonitor"/> is null.</exception>
        void Explore(ITestIsolationContext testIsolationContext, TestPackage testPackage,
            TestExplorationOptions testExplorationOptions,
            IMessageSink messageSink, IProgressMonitor progressMonitor);

        /// <summary>
        /// Runs tests from a test package.
        /// </summary>
        /// <param name="testIsolationContext">The test isolation context.</param>
        /// <param name="testPackage">The test package.</param>
        /// <param name="testExplorationOptions">The test exploration options.</param>
        /// <param name="testExecutionOptions">The test execution options.</param>
        /// <param name="messageSink">The message sink to receive test exploration and execution messages.</param>
        /// <param name="progressMonitor">The progress monitor.</param>
        /// <returns>The test report.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="testIsolationContext"/>,
        /// <paramref name="testPackage"/>, <paramref name="testExplorationOptions"/>, <paramref name="testExecutionOptions"/>,
        /// <paramref name="messageSink"/> or <paramref name="progressMonitor"/> is null.</exception>
        void Run(ITestIsolationContext testIsolationContext, TestPackage testPackage,
            TestExplorationOptions testExplorationOptions, TestExecutionOptions testExecutionOptions,
            IMessageSink messageSink, IProgressMonitor progressMonitor);
    }
}
