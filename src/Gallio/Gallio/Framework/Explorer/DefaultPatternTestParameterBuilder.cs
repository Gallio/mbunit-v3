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
using System.Collections.Generic;

namespace Gallio.Framework.Explorer
{
    /// <summary>
    /// A default pattern test parameter builder implementation.
    /// </summary>
    /// <seealso cref="PatternTestFramework"/>
    public class DefaultPatternTestParameterBuilder : IPatternTestParameterBuilder
    {
        private readonly IPatternTestBuilder testBuilder;
        private readonly PatternTestParameter testParameter;
        private List<KeyValuePair<int, Action<IPatternTestParameterBuilder>>> decorators;

        /// <summary>
        /// Creates a test parameter builder.
        /// </summary>
        /// <param name="testBuilder">The test builder</param>
        /// <param name="testParameter">The test parameter</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="testBuilder"/>
        /// or <paramref name="testParameter"/> is null</exception>
        public DefaultPatternTestParameterBuilder(IPatternTestBuilder testBuilder, PatternTestParameter testParameter)
        {
            if (testBuilder == null)
                throw new ArgumentNullException("testBuilder");
            if (testParameter == null)
                throw new ArgumentNullException("testParameter");

            this.testBuilder = testBuilder;
            this.testParameter = testParameter;
        }

        /// <inheritdoc />
        public PatternTestParameter TestParameter
        {
            get { return testParameter; }
        }

        /// <inheritdoc />
        public IPatternTestBuilder TestBuilder
        {
            get { return testBuilder; }
        }

        /// <inheritdoc />
        public IPatternTestModelBuilder TestModelBuilder
        {
            get { return testBuilder.TestModelBuilder; }
        }

        /// <inheritdoc />
        public void AddDecorator(int order, Action<IPatternTestParameterBuilder> decorator)
        {
            if (decorators == null)
                decorators = new List<KeyValuePair<int, Action<IPatternTestParameterBuilder>>>();

            decorators.Add(new KeyValuePair<int, Action<IPatternTestParameterBuilder>>(order, decorator));
        }

        /// <inheritdoc />
        public void ApplyDecorators()
        {
            if (decorators == null)
                return;

            decorators.Sort(delegate(KeyValuePair<int, Action<IPatternTestParameterBuilder>> x,
                KeyValuePair<int, Action<IPatternTestParameterBuilder>> y)
                {
                    return x.Key.CompareTo(y.Key);
                });

            foreach (KeyValuePair<int, Action<IPatternTestParameterBuilder>> entry in decorators)
                entry.Value(this);

            decorators = null;
        }
    }
}