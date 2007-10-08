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
using MbUnit.Model;

namespace MbUnit.Framework.Kernel.Attributes
{
    /// <summary>
    /// <para>
    /// Generates a fixture template from the annotated class and sets its
    /// <see cref="ITemplate.IsGenerator" /> property to true.  Subclasses
    /// can contribute actions to the template to govern how test generation
    /// takes place.  By default, the generated fixture will do nothing
    /// besides running the tests that it contains.
    /// </para>
    /// <para>
    /// At most one attribute of this type may appear on any given class.
    /// </para>
    /// </summary>
    /// <seealso cref="TypeDecoratorPatternAttribute"/>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple=false, Inherited=true)]
    public abstract class FixturePatternAttribute : TypePatternAttribute
    {
        /// <inheritdoc />
        public override void Apply(TemplateTreeBuilder builder, MbUnitTypeTemplate typeTemplate)
        {
            base.Apply(builder, typeTemplate);

            typeTemplate.IsGenerator = true;
        }
    }
}
