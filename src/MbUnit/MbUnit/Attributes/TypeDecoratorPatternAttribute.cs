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
using MbUnit.Model;

namespace MbUnit.Attributes
{
    /// <summary>
    /// <para>
    /// A type decorator pattern attribute applies various contributions to an
    /// existing type template.
    /// </para>
    /// <para>
    /// When the attribute is applied to an assembly, it affects each of the type templates
    /// within that assembly's template individually.
    /// </para>
    /// </summary>
    /// <seealso cref="TypePatternAttribute"/>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Assembly,
        AllowMultiple = true, Inherited = true)]
    public abstract class TypeDecoratorPatternAttribute : DecoratorPatternAttribute
    {
        /// <summary>
        /// Applies contributions to a type template.
        /// This method is called after the contributions of the <see cref="TypePatternAttribute"/>
        /// have been applied but before any further processing takes place.
        /// </summary>
        /// <remarks>
        /// A typical use of this method is to apply additional metadata to model
        /// objects in the template tree and to further expand the tree using
        /// declarative metadata derived via reflection.
        /// </remarks>
        /// <param name="builder">The builder</param>
        /// <param name="typeTemplate">The type template</param>
        public virtual void Apply(MbUnitTestBuilder builder, MbUnitTypeTemplate typeTemplate)
        {
        }
    }
}
