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
using Gallio.Common;
using Gallio.Common.Reflection;
using Gallio.Model.Tree;
using Gallio.Runtime.Loader;

namespace Gallio.Framework.Pattern
{
    /// <summary>
    /// An implementation of a test model builder that is used for reflection only and
    /// does not actually support building tests.
    /// </summary>
    public class ReflectionOnlyTestModelBuilder : ITestModelBuilder
    {
        private readonly IReflectionPolicy reflectionPolicy;

        /// <summary>
        /// Creates a test model builder.
        /// </summary>
        /// <param name="reflectionPolicy">The reflection policy.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="reflectionPolicy"/> is null.</exception>
        public ReflectionOnlyTestModelBuilder(IReflectionPolicy reflectionPolicy)
        {
            if (reflectionPolicy == null)
                throw new ArgumentNullException("reflectionPolicy");

            this.reflectionPolicy = reflectionPolicy;
        }

        /// <inheritdoc />
        public IReflectionPolicy ReflectionPolicy
        {
            get { return reflectionPolicy; }
        }

        void ITestModelBuilder.AddAnnotation(Annotation annotation)
        {
            throw new NotSupportedException();
        }

        void ITestModelBuilder.AddAssemblyResolver(IAssemblyResolver resolver)
        {
            throw new NotSupportedException();
        }

        void ITestModelBuilder.PublishExceptionAsAnnotation(ICodeElementInfo codeElement, Exception ex)
        {
            throw new NotSupportedException();
        }

        ITestBuilder ITestModelBuilder.CreateTopLevelTest(string name, ICodeElementInfo codeElement, ITestDataContextBuilder dataContextBuilder)
        {
            throw new NotSupportedException();
        }

        PatternTestModel ITestModelBuilder.ToTestModel()
        {
            throw new NotSupportedException();
        }

        void ISupportDeferredActions.AddDeferredAction(ICodeElementInfo codeElement, int order, Action deferredAction)
        {
            throw new NotSupportedException();
        }

        void ISupportDeferredActions.ApplyDeferredActions()
        {
            throw new NotSupportedException();
        }
    }
}
