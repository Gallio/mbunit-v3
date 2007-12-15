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
using Gallio.Model;
using Gallio.Model.Reflection;

namespace MbUnit.Model.Builder
{
    /// <summary>
    /// A test model builder maintains state that is needed during test enumeration
    /// while the <see cref="TestModel" /> is being populated.
    /// </summary>
    public interface ITestModelBuilder
    {
        /// <summary>
        /// Gets the test model being built.
        /// </summary>
        TestModel TestModel { get; }

        /// <summary>
        /// Gets the reflection policy for building the model.
        /// </summary>
        IReflectionPolicy ReflectionPolicy { get; }

        /// <summary>
        /// Gets a test builder for the root test of a particular version
        /// of the MbUnit version.
        /// </summary>
        /// <param name="frameworkVersion">The MbUnit framework version</param>
        /// <returns>The test builder</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="frameworkVersion"/> is null</exception>
        ITestBuilder GetFrameworkTestBuilder(Version frameworkVersion);

        /// <summary>
        /// Creates a test builder for a new test and registers the
        /// test with the test model so that it can be resolved later
        /// by <see cref="GetTestBuilders" />.
        /// </summary>
        /// <param name="test">The test for which to create a builder</param>
        /// <returns>The new test builder</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="test"/> is null</exception>
        ITestBuilder CreateTestBuilder(MbUnitTest test);

        /// <summary>
        /// Creates a test parameter builder for a new test parameter and registers the
        /// test parameter with the test model so that it can be resolved
        /// by <see cref="GetTestParameterBuilders" />.
        /// </summary>
        /// <param name="testParameter">The test parameter for which to create a builder</param>
        /// <returns>The new test parameter builder</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="testParameter"/> is null</exception>
        ITestParameterBuilder CreateTestParameterBuilder(MbUnitTestParameter testParameter);

        /// <summary>
        /// Finds tests that are associated with the specified <see cref="ICodeElementInfo" />
        /// and returns an enumeration of their <see cref="ITestBuilder"/> objects.
        /// </summary>
        /// <param name="codeElement">The code element</param>
        /// <returns>The enumeration of test builders</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="codeElement"/> is null</exception>
        IEnumerable<ITestBuilder> GetTestBuilders(ICodeElementInfo codeElement);

        /// <summary>
        /// Finds test parameters that are associated with the specified <see cref="ICodeElementInfo" />
        /// and returns an enumeration of their <see cref="ITestParameterBuilder"/> objects.
        /// </summary>
        /// <param name="codeElement">The code element</param>
        /// <returns>The enumeration of test parameter builders</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="codeElement"/> is null</exception>
        IEnumerable<ITestParameterBuilder> GetTestParameterBuilders(ICodeElementInfo codeElement);
    }
}
