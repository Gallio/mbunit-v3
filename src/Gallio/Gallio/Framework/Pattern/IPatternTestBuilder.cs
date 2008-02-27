// Copyright 2008 MbUnit Project - http://www.mbunit.com/
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
using Gallio.Model;

namespace Gallio.Framework.Pattern
{
    /// <summary>
    /// A pattern test builder provides the foundation for incrementally applying
    /// contributions to a <see cref="PatternTest" />.
    /// </summary>
    /// <seealso cref="PatternTestFramework"/>
    public interface IPatternTestBuilder
    {
        /// <summary>
        /// Gets the test being built.
        /// </summary>
        PatternTest Test { get; }

        /// <summary>
        /// Gets the builder for the test model.
        /// </summary>
        IPatternTestModelBuilder TestModelBuilder { get; }

        /// <summary>
        /// Adds a test as a child of this test and returns a new <see cref="IPatternTestBuilder" />.
        /// </summary>
        /// <param name="test">The test for which to create a builder</param>
        /// <returns>The new test builder</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="test"/> is null</exception>
        IPatternTestBuilder AddChild(PatternTest test);

        /// <summary>
        /// Adds a test parameter to this test and returns a new <see cref="IPatternTestParameterBuilder" />.
        /// </summary>
        /// <param name="testParameter">The test parameter for which to create a builder</param>
        /// <returns>The new test parameter builder</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="testParameter"/> is null</exception>
        IPatternTestParameterBuilder AddParameter(PatternTestParameter testParameter);

        /// <summary>
        /// Adds a test dependency.
        /// </summary>
        /// <param name="test">The test dependency to add</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="test"/> is null</exception>
        void AddDependency(ITest test);

        /// <summary>
        /// Registers a test decorator action.
        /// </summary>
        /// <param name="order">The order in which the decorator should be evaluated,
        /// decorators with lower order indices are evaluated before those with
        /// higher ones</param>
        /// <param name="decorator">The decorator action</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="decorator"/> is null</exception>
        void AddDecorator(int order, Action<IPatternTestBuilder> decorator);

        /// <summary>
        /// Applies pending decorators and finishes building the test.
        /// </summary>
        void ApplyDecorators();
    }
}
