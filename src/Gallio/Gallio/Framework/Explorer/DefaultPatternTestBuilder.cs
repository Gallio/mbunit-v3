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
using Gallio.Model;

namespace Gallio.Framework.Explorer
{
    /// <summary>
    /// A default pattern test builder implementation.
    /// </summary>
    /// <seealso cref="PatternTestFramework"/>
    public class DefaultPatternTestBuilder : IPatternTestBuilder
    {
        private readonly IPatternTestModelBuilder testModelBuilder;
        private readonly PatternTest test;
        private List<KeyValuePair<int, Action<IPatternTestBuilder>>> decorators;

        /// <summary>
        /// Creates a test builder.
        /// </summary>
        /// <param name="testModelBuilder">The test model builder</param>
        /// <param name="test">The test</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="testModelBuilder"/>
        /// or <paramref name="test"/> is null</exception>
        public DefaultPatternTestBuilder(IPatternTestModelBuilder testModelBuilder, PatternTest test)
        {
            if (testModelBuilder == null)
                throw new ArgumentNullException("testModelBuilder");
            if (test == null)
                throw new ArgumentNullException("test");

            this.testModelBuilder = testModelBuilder;
            this.test = test;
        }

        /// <inheritdoc />
        public PatternTest Test
        {
            get { return test; }
        }

        /// <inheritdoc />
        public IPatternTestModelBuilder TestModelBuilder
        {
            get { return testModelBuilder; }
        }

        /// <inheritdoc />
        public IPatternTestBuilder AddChild(PatternTest test)
        {
            Test.AddChild(test);

            IPatternTestBuilder testBuilder = new DefaultPatternTestBuilder(testModelBuilder, test);
            testModelBuilder.RegisterTestBuilder(testBuilder);
            return testBuilder;
        }

        /// <inheritdoc />
        public IPatternTestParameterBuilder AddParameter(PatternTestParameter testParameter)
        {
            Test.AddParameter(testParameter);

            IPatternTestParameterBuilder testParameterBuilder = new DefaultPatternTestParameterBuilder(this, testParameter);
            testModelBuilder.RegisterTestParameterBuilder(testParameterBuilder);
            return testParameterBuilder;
        }

        /// <inheritdoc />
        public void AddDependency(ITest test)
        {
            Test.AddDependency(test);
        }

        /// <inheritdoc />
        public void AddDecorator(int order, Action<IPatternTestBuilder> decorator)
        {
            if (decorators == null)
                decorators = new List<KeyValuePair<int, Action<IPatternTestBuilder>>>();

            decorators.Add(new KeyValuePair<int, Action<IPatternTestBuilder>>(order, decorator));
        }

        /// <inheritdoc />
        public void ApplyDecorators()
        {
            if (decorators == null)
                return;

            decorators.Sort(delegate(KeyValuePair<int, Action<IPatternTestBuilder>> x,
                KeyValuePair<int, Action<IPatternTestBuilder>> y)
            {
                return x.Key.CompareTo(y.Key);
            });

            foreach (KeyValuePair<int, Action<IPatternTestBuilder>> entry in decorators)
                entry.Value(this);

            decorators = null;
        }
    }
}
