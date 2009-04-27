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
using Gallio.Reflection;

namespace Gallio.Model
{
    /// <summary>
    /// Provides test framework services for exploring tests.
    /// </summary>
    public interface ITestExplorer
    {
        /// <summary>
        /// Applies test framework configuration options to a test domain before it is loaded.
        /// </summary>
        /// <param name="testDomainSetup">The test domain setup to modify</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="testDomainSetup"/> is null</exception>
        void ConfigureTestDomain(TestDomainSetup testDomainSetup);

        /// <summary>
        /// Returns true if the code element represents a test.
        /// </summary>
        /// <param name="reflectionPolicy">The reflection policy</param>
        /// <param name="codeElement">The code element</param>
        /// <returns>True if the code element represents a test</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="reflectionPolicy"/>
        /// or <paramref name="codeElement"/> is null</exception>
        bool IsTest(IReflectionPolicy reflectionPolicy, ICodeElementInfo codeElement);

        /// <summary>
        /// Returns true if the code element represents a part of a test such as a setup or teardown method.
        /// </summary>
        /// <param name="reflectionPolicy">The reflection policy</param>
        /// <param name="codeElement">The code element</param>
        /// <returns>True if the code element represents a part of a test</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="reflectionPolicy"/>
        /// or <paramref name="codeElement"/> is null</exception>
        bool IsTestPart(IReflectionPolicy reflectionPolicy, ICodeElementInfo codeElement);

        /// <summary>
        /// Explores the tests defined within the specified test source and populates
        /// the model with them.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The test explorer scans a volume of code using reflection to build a
        /// partial or complete test tree.
        /// </para>
        /// <para>
        /// As the test explorer explores the test source, it incrementally populates a
        /// <see cref="TestModel" /> with its discoveries.  It also optionally
        /// invokes the consumer delegate to signal when top-level tests have been found
        /// within the specified test assemblies, types or files.  When the process completes
        /// the <see cref="TestModel" /> will contain all of the tests from the resources that
        /// were explicitly specified in the test source, but it may also contain other tests
        /// that were discovered within other related resources due to implementation details.
        /// </para>
        /// </remarks>
        /// <param name="testModel">The test model to populate</param>
        /// <param name="testSource">The test source to explore</param>
        /// <param name="consumer">An action to perform on each top-level test discovered from each
        /// source, or null if no action is required</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="testModel"/>,
        /// <paramref name="testSource"/> or <paramref name="consumer"/> is null</exception>
        void Explore(TestModel testModel, TestSource testSource, Action<ITest> consumer);
    }
}
