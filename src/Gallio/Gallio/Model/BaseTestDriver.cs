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
using Gallio.Common.Reflection;
using Gallio.Common.Messaging;
using Gallio.Model.Isolation;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.Common.Collections;

namespace Gallio.Model
{
    /// <summary>
    /// A base test driver that does nothing.
    /// </summary>
    public abstract class BaseTestDriver : ITestDriver
    {
        /// <inheritdoc />
        public IList<TestPart> GetTestParts(IReflectionPolicy reflectionPolicy, ICodeElementInfo codeElement)
        {
            if (reflectionPolicy == null)
                throw new ArgumentNullException("reflectionPolicy");
            if (codeElement == null)
                throw new ArgumentNullException("codeElement");

            return GetTestPartsImpl(reflectionPolicy, codeElement);
        }

        /// <inheritdoc />
        public void Describe(IReflectionPolicy reflectionPolicy, IList<ICodeElementInfo> codeElements, TestExplorationOptions testExplorationOptions, IMessageSink messageSink, IProgressMonitor progressMonitor)
        {
            if (reflectionPolicy == null)
                throw new ArgumentNullException("reflectionPolicy");
            if (codeElements == null || codeElements.Contains(null))
                throw new ArgumentNullException("codeElements");
            if (testExplorationOptions == null)
                throw new ArgumentNullException("testExplorationOptions");
            if (messageSink == null)
                throw new ArgumentNullException("messageSink");
            if (progressMonitor == null)
                throw new ArgumentNullException("progressMonitor");

            DescribeImpl(reflectionPolicy, codeElements, testExplorationOptions, messageSink, progressMonitor);
        }

        /// <inheritdoc />
        public void Explore(ITestIsolationContext testIsolationContext, TestPackage testPackage, TestExplorationOptions testExplorationOptions,
            IMessageSink messageSink, IProgressMonitor progressMonitor)
        {
            if (testIsolationContext == null)
                throw new ArgumentNullException("testIsolationContext");
            if (testPackage == null)
                throw new ArgumentNullException("testPackage");
            if (testExplorationOptions == null)
                throw new ArgumentNullException("testExplorationOptions");
            if (messageSink == null)
                throw new ArgumentNullException("messageSink");
            if (progressMonitor == null)
                throw new ArgumentNullException("progressMonitor");

            ExploreImpl(testIsolationContext, testPackage, testExplorationOptions, messageSink, progressMonitor);
        }

        /// <inheritdoc />
        public void Run(ITestIsolationContext testIsolationContext, TestPackage testPackage, TestExplorationOptions testExplorationOptions,
            TestExecutionOptions testExecutionOptions, IMessageSink messageSink, IProgressMonitor progressMonitor)
        {
            if (testIsolationContext == null)
                throw new ArgumentNullException("testIsolationContext");
            if (testPackage == null)
                throw new ArgumentNullException("testPackage");
            if (testExplorationOptions == null)
                throw new ArgumentNullException("testExplorationOptions");
            if (testExecutionOptions == null)
                throw new ArgumentNullException("testExecutionOptions");
            if (messageSink == null)
                throw new ArgumentNullException("messageSink");
            if (progressMonitor == null)
                throw new ArgumentNullException("progressMonitor");

            RunImpl(testIsolationContext, testPackage, testExplorationOptions, testExecutionOptions, messageSink, progressMonitor);
        }

        /// <summary>
        /// Gets the test parts represented by a code element.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The default implementation always returns an empty array.  Subclasses may override
        /// this method to provide information about code elements that represent tests.
        /// </para>
        /// </remarks>
        /// <param name="reflectionPolicy">The reflection policy, not null.</param>
        /// <param name="codeElement">The code element, not null.</param>
        /// <returns>The test parts, or an empty array if none.</returns>
        protected virtual IList<TestPart> GetTestPartsImpl(IReflectionPolicy reflectionPolicy, ICodeElementInfo codeElement)
        {
            return EmptyArray<TestPart>.Instance;
        }

        /// <summary>
        /// Describes tests via reflection over code elements.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The default implementation does nothing.  Subclasses may override to enable tests
        /// to be described via reflection only.  This is required for ReSharper integration.
        /// </para>
        /// </remarks>
        /// <param name="reflectionPolicy">The reflection policy, not null.</param>
        /// <param name="codeElements">The enumeration of code elements, usually <see cref="ITypeInfo" />
        /// and <see cref="IAssemblyInfo" /> objects, that might contain tests to be described, not null.</param>
        /// <param name="testExplorationOptions">The test exploration options, not null.</param>
        /// <param name="messageSink">The message sink to receive test exploration messages, not null.</param>
        /// <param name="progressMonitor">The progress monitor, not null.</param>
        protected virtual void DescribeImpl(IReflectionPolicy reflectionPolicy, IList<ICodeElementInfo> codeElements,
            TestExplorationOptions testExplorationOptions, IMessageSink messageSink, IProgressMonitor progressMonitor)
        {
        }

        /// <summary>
        /// Explores tests from a test package.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The default implementation does nothing.  Subclasses may override to enable tests
        /// to be explored.  This is required for presenting lists of tests in test runners like Icarus.
        /// </para>
        /// </remarks>
        /// <param name="testIsolationContext">The test isolation context, not null.</param>
        /// <param name="testPackage">The test package, not null.</param>
        /// <param name="testExplorationOptions">The test exploration options, not null.</param>
        /// <param name="messageSink">The message sink to receive test exploration messages, not null.</param>
        /// <param name="progressMonitor">The progress monitor, not null.</param>
        /// <returns>The test report.</returns>
        protected virtual void ExploreImpl(ITestIsolationContext testIsolationContext, TestPackage testPackage,
            TestExplorationOptions testExplorationOptions,
            IMessageSink messageSink, IProgressMonitor progressMonitor)
        {
        }

        /// <summary>
        /// Runs tests from a test package.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The default implementation does nothing.  Subclasses may override to enable tests
        /// to be run.  This is required for actually running tests.
        /// </para>
        /// </remarks>
        /// <param name="testIsolationContext">The test isolation context, not null.</param>
        /// <param name="testPackage">The test package, not null.</param>
        /// <param name="testExplorationOptions">The test exploration options, not null.</param>
        /// <param name="testExecutionOptions">The test execution options, not null.</param>
        /// <param name="messageSink">The message sink to receive test exploration and execution messages, not null.</param>
        /// <param name="progressMonitor">The progress monitor, not null.</param>
        /// <returns>The test report.</returns>
        protected virtual void RunImpl(ITestIsolationContext testIsolationContext, TestPackage testPackage,
            TestExplorationOptions testExplorationOptions, TestExecutionOptions testExecutionOptions,
            IMessageSink messageSink, IProgressMonitor progressMonitor)
        {
        }
    }
}
