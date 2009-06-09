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
using Gallio.Framework.Pattern;
using Gallio.Model;

namespace MbUnit.Framework
{
    /// <summary>
    /// Associates an annotation message of the specified type with the code element.
    /// </summary>
    /// <remarks>
    /// <para>
    /// An annotation is a message that is associated with code. This is different
    /// from the metadata that may be associated with a test because an annotation
    /// can apply to code elements that are not tests.
    /// </para>
    /// <para>
    /// For example, if an error occurs while parsing the declarative attribute 
    /// structure that defines a test, then the test itself may not be added to
    /// the test tree.  Instead, the Pattern Test Framework will associated an
    /// error annotation with the point of definition of the test.  This annotation
    /// is displayed by the test runner as an error.  Some test runners, such as the
    /// ReSharper test runner, will actually highlight the annotated element in the
    /// IDE using an error stripe.
    /// </para>
    /// <para>
    /// This attribute makes it possible to add annotations to code elements
    /// declaratively.
    /// </para>
    /// <para>
    /// Note: The annotation will only be added if the associated code element is
    /// traversed during the normal sequence of test exploration.
    /// </para>
    /// </remarks>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
    public class AnnotationAttribute : AnnotationPatternAttribute
    {
        /// <summary>
        /// Associates an annotation message of the specified type with the code element.
        /// </summary>
        /// <param name="type">The annotation type.</param>
        /// <param name="message">The annotation message.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="message"/> is null.</exception>
        public AnnotationAttribute(AnnotationType type, string message)
            : base(type, message)
        {
        }
    }
}
