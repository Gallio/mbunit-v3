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
using MbUnit.Model.Patterns;

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
        private readonly IPatternResolver patternResolver;

        private readonly Dictionary<Version, ITestBuilder> frameworkTestBuilders;
        private readonly MultiMap<ICodeElementInfo, ITestBuilder> testBuilders;
        private readonly MultiMap<ICodeElementInfo, ITestParameterBuilder> testParameterBuilders;

        /// <summary>
        /// Creates a test model builder.
        /// </summary>
        /// <param name="testModel">The test model to build on</param>
        /// <param name="patternResolver">The pattern resolver</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="testModel"/> or
        /// <paramref name="patternResolver"/> is null</exception>
        public DefaultTestModelBuilder(TestModel testModel, IPatternResolver patternResolver)
        {
            if (testModel == null)
                throw new ArgumentNullException("testModel");
            if (patternResolver == null)
                throw new ArgumentNullException("patternResolver");

            this.testModel = testModel;
            this.patternResolver = patternResolver;

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
            get { return testModel.TestPackage.ReflectionPolicy; }
        }

        /// <inheritdoc />
        public IPatternResolver PatternResolver
        {
            get { return patternResolver; }
        }

        /// <inheritdoc />
        public ITestBuilder GetFrameworkTestBuilder(Version frameworkVersion)
        {
            if (frameworkVersion == null)
                throw new ArgumentNullException("frameworkVersion");

            ITestBuilder frameworkTestBuilder;
            if (! frameworkTestBuilders.TryGetValue(frameworkVersion, out frameworkTestBuilder))
            {
                MbUnitFrameworkTest frameworkTest = new MbUnitFrameworkTest(frameworkVersion);

                frameworkTestBuilder = new DefaultTestBuilder(this, frameworkTest);
                frameworkTestBuilders.Add(frameworkVersion, frameworkTestBuilder);

                testModel.RootTest.AddChild(frameworkTest);
            }

            return frameworkTestBuilder;
        }

        /// <inheritdoc />
        public void RegisterTestBuilder(ITestBuilder testBuilder)
        {
            if (testBuilder == null)
                throw new ArgumentNullException("testBuilder");

            ICodeElementInfo codeElement = testBuilder.Test.CodeElement;
            if (codeElement != null)
                testBuilders.Add(codeElement, testBuilder);
        }

        /// <inheritdoc />
        public void RegisterTestParameterBuilder(ITestParameterBuilder testParameterBuilder)
        {
            if (testParameterBuilder == null)
                throw new ArgumentNullException("testParameterBuilder");

            ICodeElementInfo codeElement = testParameterBuilder.TestParameter.CodeElement;
            if (codeElement != null)
                testParameterBuilders.Add(codeElement, testParameterBuilder);
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
