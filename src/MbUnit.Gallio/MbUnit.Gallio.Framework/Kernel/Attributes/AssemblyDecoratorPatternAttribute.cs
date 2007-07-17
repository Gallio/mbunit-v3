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
    /// An assembly decorator pattern attribute applies various contributions to an
    /// existing assembly template model object.
    /// </summary>
    /// <seealso cref="AssemblyPatternAttribute"/>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple=true, Inherited=true)]
    public abstract class AssemblyDecoratorPatternAttribute : DecoratorPatternAttribute
    {
        /// <summary>
        /// Applies contributions to an assembly template.
        /// This method is called after the contributions of the <see cref="AssemblyPatternAttribute"/>
        /// have been applied but before any further processing takes place.
        /// </summary>
        /// <remarks>
        /// A typical use of this method is to apply additional metadata to model
        /// objects in the test template tree and to further expand the tree using
        /// declarative metadata derived via reflection.
        /// </remarks>
        /// <param name="builder">The test template tree builder</param>
        /// <param name="assemblyTemplate">The assembly template</param>
        public virtual void Apply(TestTemplateTreeBuilder builder, MbUnitTestAssemblyTemplate assemblyTemplate)
        {
        }

        /// <summary>
        /// Processes all assembly decorators via reflection.
        /// </summary>
        /// <param name="builder">The test template tree builder</param>
        /// <param name="assemblyTemplate">The assembly template</param>
        /// <param name="attributeProvider">The attribute provider to scan</param>
        public static void ProcessDecorators(TestTemplateTreeBuilder builder, MbUnitTestAssemblyTemplate assemblyTemplate, ICustomAttributeProvider attributeProvider)
        {
            object[] decorators = attributeProvider.GetCustomAttributes(typeof(AssemblyDecoratorPatternAttribute), true);
            Sort(decorators);

            foreach (AssemblyDecoratorPatternAttribute decoratorAttribute in decorators)
            {
                decoratorAttribute.Apply(builder, assemblyTemplate);
            }
        }
    }
}
