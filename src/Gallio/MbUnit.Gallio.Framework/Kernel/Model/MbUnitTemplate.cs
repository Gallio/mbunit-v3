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
using MbUnit.Framework.Kernel.DataBinding;
using MbUnit.Framework.Kernel.Model;

namespace MbUnit.Framework.Kernel.Model
{
    /// <summary>
    /// Abstract base class for MbUnit-derived templates.
    /// </summary>
    public abstract class MbUnitTemplate : BaseTemplate
    {
        /// <summary>
        /// Initializes a template initially without a parent.
        /// </summary>
        /// <param name="name">The name of the component</param>
        /// <param name="codeReference">The point of definition</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/>
        /// or <paramref name="codeReference"/> is null</exception>
        public MbUnitTemplate(string name, CodeReference codeReference)
            : base(name, codeReference)
        {
        }

        /// <summary>
        /// This event is fired when a new template binding is created to allow
        /// MbUnit framework attributes to contribute to the test construction
        /// process performed by the binding.  Framework attributes will generally
        /// hook this event to capture the binding process then hook additional
        /// events on the bound template itself which is passed in as the parameter.
        /// </summary>
        public event Action<ITemplateBinding> ProcessBinding;

        /// <inheritdoc />
        public override ITemplateBinding Bind(TemplateBindingScope scope, IDictionary<ITemplateParameter, IDataFactory> arguments)
        {
            MbUnitTemplateBinding binding = new MbUnitTemplateBinding(this, scope, arguments);

            if (ProcessBinding != null)
                ProcessBinding(binding);

            return binding;
        }
    }
}
