// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
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
using Gallio.Common.Reflection;
using Gallio.Model.Tree;
using Gallio.Runtime.Loader;

namespace Gallio.Framework.Pattern
{
    /// <summary>
    /// A test model builder applies contributions to a test model under construction.
    /// </summary>
    public interface ITestModelBuilder : ISupportDeferredActions
    {
        /// <summary>
        /// Gets the reflection policy for the model.
        /// </summary>
        IReflectionPolicy ReflectionPolicy { get; }

        /// <summary>
        /// Gets the builder of the root test.
        /// </summary>
        /// <returns>The builder for the root test.</returns>
        ITestBuilder RootTestBuilder { get; }

        /// <summary>
        /// Adds an annotation to the test model.
        /// </summary>
        /// <param name="annotation">The annotation to add.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="annotation"/> is null.</exception>
        void AddAnnotation(Annotation annotation);

        /// <summary>
        /// Adds an assembly resolver to the test model's assembly loading policy.
        /// </summary>
        /// <param name="resolver">The resolver to add.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="resolver"/> is null.</exception>
        void AddAssemblyResolver(IAssemblyResolver resolver);

        /// <summary>
        /// Publishes an exception as an annotation about a particular code element.
        /// </summary>
        /// <param name="codeElement">The code element, or null if none.</param>
        /// <param name="ex">The exception to publish.</param>
        void PublishExceptionAsAnnotation(ICodeElementInfo codeElement, Exception ex);

        /// <summary>
        /// Gets the underlying test model.
        /// </summary>
        /// <returns>The underlying test model.</returns>
        PatternTestModel ToTestModel();
    }
}
