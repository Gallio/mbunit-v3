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

namespace MbUnit.Model.Builder
{
    /// <summary>
    /// A test parameter builder provides the foundation for incrementally applying
    /// contributions to a <see cref="MbUnitTestParameter" />.
    /// </summary>
    public interface ITestParameterBuilder
    {
        /// <summary>
        /// Gets the test parameter being built.
        /// </summary>
        MbUnitTestParameter TestParameter { get; }

        /// <summary>
        /// Gets the builder for the test that owns this parameter.
        /// </summary>
        ITestBuilder TestBuilder { get; }

        /// <summary>
        /// Gets the builder for the test model.
        /// </summary>
        ITestModelBuilder TestModelBuilder { get; }

        /// <summary>
        /// Registers a test parameter decorator action.
        /// </summary>
        /// <param name="order">The order in which the decorator should be evaluated,
        /// decorators with lower order indices are evaluated before those with
        /// higher ones</param>
        /// <param name="decorator">The decorator action</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="decorator"/> is null</exception>
        void AddDecorator(int order, Action<ITestParameterBuilder> decorator);

        /// <summary>
        /// Applies pending decorators and finishes building the test parameter.
        /// </summary>
        void ApplyDecorators();
    }
}