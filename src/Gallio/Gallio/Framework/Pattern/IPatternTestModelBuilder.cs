// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
using Gallio.Reflection;
using Gallio.Framework.Pattern;

namespace Gallio.Framework.Pattern
{
    /// <summary>
    /// A pattern test model builder maintains state that is needed during test enumeration
    /// while the <see cref="TestModel" /> is being populated with <see cref="PatternTest" />
    /// objects by the <see cref="PatternTestExplorer" />.
    /// </summary>
    /// <seealso cref="PatternTestFramework"/>
    public interface IPatternTestModelBuilder
    {
        /// <summary>
        /// Gets the test model being built.
        /// </summary>
        TestModel TestModel { get; }

        /// <summary>
        /// Gets the reflection policy for the model.
        /// </summary>
        IReflectionPolicy ReflectionPolicy { get; }

        /// <summary>
        /// Gets the pattern resolver for the model.
        /// </summary>
        IPatternResolver PatternResolver { get; }

        /// <summary>
        /// Adds a top level test and returns a new <see cref="IPatternTestBuilder" />.
        /// </summary>
        /// <param name="test">The test for which to create a builder</param>
        /// <returns>The new test builder</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="test"/> is null</exception>
        IPatternTestBuilder AddTopLevelTest(PatternTest test);

        /// <summary>
        /// Registers the test builder with the test model so that it can be resolved later
        /// by <see cref="GetTestBuilders" />.
        /// </summary>
        /// <param name="testBuilder">The test builder</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="testBuilder"/> is null</exception>
        void RegisterTestBuilder(IPatternTestBuilder testBuilder);

        /// <summary>
        /// Registers the test parameter builder with the test model so that it can be resolved
        /// by <see cref="GetTestParameterBuilders" />.
        /// </summary>
        /// <param name="testParameterBuilder">The test parameter builder</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="testParameterBuilder"/> is null</exception>
        void RegisterTestParameterBuilder(IPatternTestParameterBuilder testParameterBuilder);

        /// <summary>
        /// Finds tests that are associated with the specified <see cref="ICodeElementInfo" />
        /// and returns an enumeration of their <see cref="IPatternTestBuilder"/> objects.
        /// </summary>
        /// <param name="codeElement">The code element</param>
        /// <returns>The enumeration of test builders</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="codeElement"/> is null</exception>
        IEnumerable<IPatternTestBuilder> GetTestBuilders(ICodeElementInfo codeElement);

        /// <summary>
        /// Finds test parameters that are associated with the specified <see cref="ICodeElementInfo" />
        /// and returns an enumeration of their <see cref="IPatternTestParameterBuilder"/> objects.
        /// </summary>
        /// <param name="codeElement">The code element</param>
        /// <returns>The enumeration of test parameter builders</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="codeElement"/> is null</exception>
        IEnumerable<IPatternTestParameterBuilder> GetTestParameterBuilders(ICodeElementInfo codeElement);
    }
}
