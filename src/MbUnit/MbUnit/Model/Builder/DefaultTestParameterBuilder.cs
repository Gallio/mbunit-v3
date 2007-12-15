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

namespace MbUnit.Model.Builder
{
    /// <summary>
    /// A default test parameter builder implementation.
    /// </summary>
    public class DefaultTestParameterBuilder : ITestParameterBuilder
    {
        private readonly ITestModelBuilder testModelBuilder;
        private readonly MbUnitTestParameter testParameter;
        private List<KeyValuePair<int, Action<ITestParameterBuilder>>> decorators;

        /// <summary>
        /// Creates a test parameter builder.
        /// </summary>
        /// <param name="testModelBuilder">The test model builder</param>
        /// <param name="testParameter">The test parameter</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="testModelBuilder"/>
        /// or <paramref name="testParameter"/> is null</exception>
        public DefaultTestParameterBuilder(ITestModelBuilder testModelBuilder, MbUnitTestParameter testParameter)
        {
            if (testModelBuilder == null)
                throw new ArgumentNullException("testModelBuilder");
            if (testParameter == null)
                throw new ArgumentNullException("testParameter");

            this.testModelBuilder = testModelBuilder;
            this.testParameter = testParameter;
        }

        /// <inheritdoc />
        public MbUnitTestParameter TestParameter
        {
            get { return testParameter; }
        }

        /// <inheritdoc />
        public ITestModelBuilder TestModelBuilder
        {
            get { return testModelBuilder; }
        }

        /// <inheritdoc />
        public void AddDecorator(int order, Action<ITestParameterBuilder> decorator)
        {
            if (decorators == null)
                decorators = new List<KeyValuePair<int, Action<ITestParameterBuilder>>>();

            decorators.Add(new KeyValuePair<int, Action<ITestParameterBuilder>>(order, decorator));
        }

        /// <inheritdoc />
        public void ApplyDecorators()
        {
            if (decorators == null)
                return;

            decorators.Sort(delegate(KeyValuePair<int, Action<ITestParameterBuilder>> x,
                KeyValuePair<int, Action<ITestParameterBuilder>> y)
                {
                    return x.Key.CompareTo(y.Key);
                });

            foreach (KeyValuePair<int, Action<ITestParameterBuilder>> entry in decorators)
                entry.Value(this);

            decorators = null;
        }
    }
}