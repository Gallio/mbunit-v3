// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan de Halleux
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
using System.Text;
using Gallio.Model;
using Gallio.Common.Reflection;
using Gallio.Runtime.Loader;

namespace Gallio.Framework.Pattern
{
    /// <summary>
    /// Default implementation of a test model builder.
    /// </summary>
    public class DefaultTestModelBuilder : BaseBuilder, ITestModelBuilder
    {
        private readonly TestModel testModel;

        /// <summary>
        /// Creates a test model builder.
        /// </summary>
        /// <param name="testModel">The underlying test model</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="testModel"/> is null</exception>
        public DefaultTestModelBuilder(TestModel testModel)
        {
            if (testModel == null)
                throw new ArgumentNullException("testModel");

            this.testModel = testModel;
        }

        /// <inheritdoc />
        public IReflectionPolicy ReflectionPolicy
        {
            get { return testModel.TestPackage.ReflectionPolicy; }
        }

        /// <inheritdoc />
        public void AddAnnotation(Annotation annotation)
        {
            if (annotation == null)
                throw new ArgumentNullException("annotation");

            testModel.AddAnnotation(annotation);
        }

        /// <inheritdoc />
        public void AddAssemblyResolver(IAssemblyResolver resolver)
        {
            if (resolver == null)
                throw new ArgumentNullException("resolver");

            testModel.TestPackage.Loader.AssemblyResolverManager.AddAssemblyResolver(resolver);
        }

        /// <inheritdoc />
        public void PublishExceptionAsAnnotation(ICodeElementInfo codeElement, Exception ex)
        {
            if (ex is PatternUsageErrorException)
            {
                testModel.AddAnnotation(new Annotation(AnnotationType.Error, codeElement,
                    ex.Message, ex.InnerException));
            }
            else
            {
                testModel.AddAnnotation(new Annotation(AnnotationType.Error, codeElement,
                    "An exception was thrown while exploring tests.", ex));
            }
        }

        /// <inheritdoc />
        public ITestBuilder CreateTopLevelTest(string name, ICodeElementInfo codeElement, ITestDataContextBuilder dataContextBuilder)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (dataContextBuilder == null)
                throw new ArgumentNullException("dataContextBuilder");

            PatternTest topLevelTest = new PatternTest(name, codeElement, dataContextBuilder.ToPatternTestDataContext());
            testModel.RootTest.AddChild(topLevelTest);
            return new DefaultTestBuilder(this, topLevelTest);
        }

        /// <inheritdoc />
        public TestModel ToTestModel()
        {
            return testModel;
        }

        /// <inheritdoc />
        protected sealed override ITestModelBuilder GetTestModelBuilder()
        {
            return this;
        }
    }
}
