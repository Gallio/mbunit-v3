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
using System.Reflection;
using MbUnit.Framework.Kernel.Model;
using MbUnit.Framework.Kernel.Model;

namespace MbUnit.Framework.Kernel.Attributes
{
    /// <summary>
    /// <para>
    /// A test decorator pattern attribute applies various contributions to an
    /// existing test method template model object.
    /// </para>
    /// <para>
    /// When the attribute is applied to a test fixture class, it affects each of the tests
    /// within that fixture individually.
    /// </para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public abstract class MethodDecoratorPatternAttribute : DecoratorPatternAttribute
    {
        /// <summary>
        /// Applies contributions to a method template.
        /// This method is called after the contributions of the <see cref="MethodPatternAttribute"/>
        /// have been applied but before any further processing takes place.
        /// </summary>
        /// <remarks>
        /// A typical use of this method is to apply additional metadata to model
        /// objects in the test template tree and to further expand the tree using
        /// declarative metadata derived via reflection.
        /// </remarks>
        /// <param name="builder">The test template tree builder</param>
        /// <param name="methodTemplate">The method template</param>
        public virtual void Apply(TestTemplateTreeBuilder builder, MbUnitTestMethodTemplate methodTemplate)
        {
        }

        /// <summary>
        /// Processes all test decorators via reflection.
        /// </summary>
        /// <param name="builder">The test template tree builder</param>
        /// <param name="methodTemplate">The method template</param>
        /// <param name="attributeProvider">The attribute provider to scan</param>
        public static void ProcessDecorators(TestTemplateTreeBuilder builder, MbUnitTestMethodTemplate methodTemplate, ICustomAttributeProvider attributeProvider)
        {
            object[] decorators = attributeProvider.GetCustomAttributes(typeof(MethodDecoratorPatternAttribute), true);
            Sort(decorators);

            foreach (MethodDecoratorPatternAttribute decoratorAttribute in decorators)
            {
                decoratorAttribute.Apply(builder, methodTemplate);
            }
        }
    }
}
