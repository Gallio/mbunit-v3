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
using MbUnit.Framework.Kernel.DataBinding;
using MbUnit.Framework.Kernel.Metadata;
using MbUnit.Framework.Kernel.Model;

namespace MbUnit.Plugin.MbUnit2Adapter.Core
{
    /// <summary>
    /// The MbUnit v2 assembly template.  Since MbUnit v2 does not support test
    /// parameters, we do not fully decompose the tests into a template tree.
    /// Instead, this template takes care of building all of the tests as needed.
    /// </summary>
    public class MbUnit2AssemblyTemplate : BaseTemplate
    {
        private Assembly assembly;

        /// <summary>
        /// Initializes a template initially without a parent.
        /// </summary>
        /// <param name="assembly">The assembly</param>
        public MbUnit2AssemblyTemplate(Assembly assembly)
            : base(assembly.FullName, CodeReference.CreateFromAssembly(assembly))
        {
            this.assembly = assembly;

            Kind = ComponentKind.Assembly;
        }

        /// <summary>
        /// Gets the assembly.
        /// </summary>
        public Assembly Assembly
        {
            get { return assembly; }
        }

        /// <inheritdoc />
        public override ITemplateBinding Bind(TemplateBindingScope scope, IDictionary<ITemplateParameter, IDataFactory> arguments)
        {
            return new MbUnit2AssemblyTemplateBinding(this, scope, arguments);
        }
    }
}
