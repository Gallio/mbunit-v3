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

namespace MbUnit.Model.Builder
{
    /// <summary>
    /// A default test builder implementation.
    /// </summary>
    public class DefaultTestBuilder : ITestBuilder
    {
        private readonly ITestModelBuilder testModelBuilder;
        private readonly MbUnitTest test;
        private List<KeyValuePair<int, Action<ITestBuilder>>> decorators;

        /// <summary>
        /// Creates a test builder.
        /// </summary>
        /// <param name="testModelBuilder">The test model builder</param>
        /// <param name="test">The test</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="testModelBuilder"/>
        /// or <paramref name="test"/> is null</exception>
        public DefaultTestBuilder(ITestModelBuilder testModelBuilder, MbUnitTest test)
        {
            if (testModelBuilder == null)
                throw new ArgumentNullException("testModelBuilder");
            if (test == null)
                throw new ArgumentNullException("test");

            this.testModelBuilder = testModelBuilder;
            this.test = test;
        }

        /// <inheritdoc />
        public MbUnitTest Test
        {
            get { return test; }
        }

        /// <inheritdoc />
        public ITestModelBuilder TestModelBuilder
        {
            get { return testModelBuilder; }
        }

        /// <inheritdoc />
        public ITestBuilder AddChild(MbUnitTest test)
        {
            Test.AddChild(test);

            ITestBuilder testBuilder = new DefaultTestBuilder(testModelBuilder, test);
            testModelBuilder.RegisterTestBuilder(testBuilder);
            return testBuilder;
        }

        /// <inheritdoc />
        public ITestParameterBuilder AddParameter(MbUnitTestParameter testParameter)
        {
            Test.AddParameter(testParameter);

            ITestParameterBuilder testParameterBuilder = new DefaultTestParameterBuilder(this, testParameter);
            testModelBuilder.RegisterTestParameterBuilder(testParameterBuilder);
            return testParameterBuilder;
        }

        /// <inheritdoc />
        public void AddDependency(ITest test)
        {
            Test.AddDependency(test);
        }

        /// <inheritdoc />
        public void AddDecorator(int order, Action<ITestBuilder> decorator)
        {
            if (decorators == null)
                decorators = new List<KeyValuePair<int, Action<ITestBuilder>>>();

            decorators.Add(new KeyValuePair<int, Action<ITestBuilder>>(order, decorator));
        }

        /// <inheritdoc />
        public void ApplyDecorators()
        {
            if (decorators == null)
                return;

            decorators.Sort(delegate(KeyValuePair<int, Action<ITestBuilder>> x,
                KeyValuePair<int, Action<ITestBuilder>> y)
            {
                return x.Key.CompareTo(y.Key);
            });

            foreach (KeyValuePair<int, Action<ITestBuilder>> entry in decorators)
                entry.Value(this);

            decorators = null;
        }
    }
}
