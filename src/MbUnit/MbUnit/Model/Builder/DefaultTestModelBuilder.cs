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
using Gallio.Collections;
using Gallio.Model;
using Gallio.Model.Reflection;

namespace MbUnit.Model.Builder
{
    /// <summary>
    /// The default test model builder implementation.
    /// It maintains a dictionary of tests and test parameters indexed by code element
    /// for all of the builders it was responsible for creating.
    /// </summary>
    public class DefaultTestModelBuilder : ITestModelBuilder
    {
        private readonly TestModel testModel;
        private readonly IReflectionPolicy reflectionPolicy;

        private readonly Dictionary<Version, ITestBuilder> frameworkTestBuilders;
        private readonly MultiMap<ICodeElementInfo, ITestBuilder> testBuilders;
        private readonly MultiMap<ICodeElementInfo, ITestParameterBuilder> testParameterBuilders;

        /// <summary>
        /// Creates a test model builder.
        /// </summary>
        /// <param name="testModel">The test model to build on</param>
        /// <param name="reflectionPolicy">The reflection policy</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="testModel"/>
        /// or <paramref name="reflectionPolicy"/> is null</exception>
        public DefaultTestModelBuilder(TestModel testModel, IReflectionPolicy reflectionPolicy)
        {
            if (testModel == null)
                throw new ArgumentNullException("testModel");
            if (reflectionPolicy == null)
                throw new ArgumentNullException("reflectionPolicy");

            this.testModel = testModel;
            this.reflectionPolicy = reflectionPolicy;

            frameworkTestBuilders = new Dictionary<Version, ITestBuilder>();
            testBuilders = new MultiMap<ICodeElementInfo, ITestBuilder>();
            testParameterBuilders = new MultiMap<ICodeElementInfo, ITestParameterBuilder>();
        }

        /// <inheritdoc />
        public TestModel TestModel
        {
            get { return testModel; }
        }

        /// <inheritdoc />
        public IReflectionPolicy ReflectionPolicy
        {
            get { return reflectionPolicy; }
        }

        /// <inheritdoc />
        public ITestBuilder GetFrameworkTestBuilder(Version frameworkVersion)
        {
            ITestBuilder frameworkTestBuilder;
            if (! frameworkTestBuilders.TryGetValue(frameworkVersion, out frameworkTestBuilder))
            {
                MbUnitFrameworkTest frameworkTest = new MbUnitFrameworkTest(frameworkVersion);

                frameworkTestBuilder = CreateTestBuilder(frameworkTest);
                frameworkTestBuilders.Add(frameworkVersion, frameworkTestBuilder);

                testModel.RootTest.AddChild(frameworkTest);
            }

            return frameworkTestBuilder;
        }

        /// <inheritdoc />
        public ITestBuilder CreateTestBuilder(MbUnitTest test)
        {
            ITestBuilder testBuilder = new DefaultTestBuilder(this, test);

            ICodeElementInfo codeElement = test.CodeElement;
            if (codeElement != null)
                testBuilders.Add(codeElement, testBuilder);

            return testBuilder;
        }

        /// <inheritdoc />
        public ITestParameterBuilder CreateTestParameterBuilder(MbUnitTestParameter testParameter)
        {
            ITestParameterBuilder testParameterBuilder = new DefaultTestParameterBuilder(this, testParameter);

            ICodeElementInfo codeElement = testParameter.CodeElement;
            if (codeElement != null)
                testParameterBuilders.Add(codeElement, testParameterBuilder);

            return testParameterBuilder;
        }

        /// <inheritdoc />
        public IEnumerable<ITestBuilder> GetTestBuilders(ICodeElementInfo codeElement)
        {
            return testBuilders[codeElement];
        }

        /// <inheritdoc />
        public IEnumerable<ITestParameterBuilder> GetTestParameterBuilders(ICodeElementInfo codeElement)
        {
            return testParameterBuilders[codeElement];
        }
    }
}
