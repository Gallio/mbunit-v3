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
using MbUnit.Model;

namespace MbUnit.Framework.Kernel.Attributes
{
    /// <summary>
    /// <para>
    /// A method decorator pattern attribute applies contributions to an
    /// existing method template model object.
    /// </para>
    /// <para>
    /// When the attribute is applied to a type, it affects each of the methods templates
    /// within that type's template individually.
    /// </para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = true, Inherited = true)]
    public abstract class MethodDecoratorPatternAttribute : DecoratorPatternAttribute
    {
        /// <summary>
        /// Applies contributions to a method template.
        /// This method is called after the contributions of the <see cref="MethodPatternAttribute"/>
        /// have been applied but before any further processing takes place.
        /// </summary>
        /// <remarks>
        /// A typical use of this method is to apply additional metadata to model
        /// objects in the template tree and to further expand the tree using
        /// declarative metadata derived via reflection.
        /// </remarks>
        /// <param name="builder">The template tree builder</param>
        /// <param name="methodTemplate">The method template</param>
        public virtual void Apply(TemplateTreeBuilder builder, MbUnitMethodTemplate methodTemplate)
        {
        }

        /// <summary>
        /// Processes all method decorators via reflection.
        /// </summary>
        /// <param name="builder">The template tree builder</param>
        /// <param name="methodTemplate">The method template</param>
        /// <param name="attributeProvider">The attribute provider to scan</param>
        public static void ProcessDecorators(TemplateTreeBuilder builder, MbUnitMethodTemplate methodTemplate, ICustomAttributeProvider attributeProvider)
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
