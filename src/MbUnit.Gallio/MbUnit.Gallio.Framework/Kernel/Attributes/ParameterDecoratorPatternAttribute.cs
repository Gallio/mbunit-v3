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
using System.Reflection;
using System.Text;
using MbUnit.Framework.Kernel.Model;

namespace MbUnit.Framework.Kernel.Attributes
{
    /// <summary>
    /// A parameter decorator pattern attribute applies various contributions to an
    /// existing parameter model object.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter,
        AllowMultiple = true, Inherited = true)]
    public abstract class ParameterDecoratorPatternAttribute : DecoratorPatternAttribute
    {
        /// <summary>
        /// Applies contributions to a parameter.
        /// This method is called after the contributions of the <see cref="ParameterPatternAttribute"/>
        /// have been applied but before any further processing takes place.
        /// </summary>
        /// <remarks>
        /// A typical use of this method is to apply additional metadata to model
        /// objects in the template tree and to further expand the tree using
        /// declarative metadata derived via reflection.
        /// </remarks>
        /// <param name="builder">The template tree builder</param>
        /// <param name="parameter">The parameter</param>
        public virtual void Apply(TemplateTreeBuilder builder, MbUnitTemplateParameter parameter)
        {
        }

        /// <summary>
        /// Processes all parameter decorators via reflection.
        /// </summary>
        /// <param name="builder">The template tree builder</param>
        /// <param name="parameter">The parameter</param>
        /// <param name="attributeProvider">The attribute provider to scan</param>
        public static void ProcessDecorators(TemplateTreeBuilder builder, MbUnitTemplateParameter parameter, ICustomAttributeProvider attributeProvider)
        {
            object[] decorators = attributeProvider.GetCustomAttributes(typeof(ParameterDecoratorPatternAttribute), true);
            Sort(decorators);

            foreach (ParameterDecoratorPatternAttribute decoratorAttribute in decorators)
            {
                decoratorAttribute.Apply(builder, parameter);
            }
        }
    }
}
