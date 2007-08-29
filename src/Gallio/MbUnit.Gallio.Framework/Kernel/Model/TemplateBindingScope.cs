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
using System.Text;
using MbUnit.Framework.Kernel.Collections;
using MbUnit.Framework.Kernel.DataBinding;
using MbUnit.Framework.Kernel.Utilities;

namespace MbUnit.Framework.Kernel.Model
{
    /// <summary>
    /// A template binding scope provides a mechanism for template bindings to
    /// access state provided by containing template bindings.
    /// </summary>
    public class TemplateBindingScope
    {
        private readonly TemplateBindingScope parent;

        /// <summary>
        /// Creates a template binding scope as a child of another scope.
        /// </summary>
        /// <param name="parent">The parent scope, or null if none</param>
        public TemplateBindingScope(TemplateBindingScope parent)
        {
            this.parent = parent;
        }

        /// <summary>
        /// Gets the parent scope, or null if none.
        /// </summary>
        public TemplateBindingScope Parent
        {
            get { return parent; }
        }

        /// <summary>
        /// Produces bindings of a template by producing arguments for template parameters
        /// based on the data sources available in the scope.
        /// </summary>
        /// <param name="template">The template to bind</param>
        /// <returns>The bound templates</returns>
        public IEnumerable<ITemplateBinding> Bind(ITemplate template)
        {
            TemplateBindingScope nestedScope = new TemplateBindingScope(this);

            //DataBinding.DataBinding[] bindings = ListUtils.ConvertAllToArray<ITemplateParameter, DataBinding.DataBinding>(template.Parameters, CreateDataBindingFromParameter);

            if (template.Parameters.Count == 0)
                yield return template.Bind(nestedScope, EmptyDictionary<ITemplateParameter, IDataFactory>.Instance);
        }

        /*
        private DataBinding.DataBinding CreateDataBindingFromParameter(ITemplateParameter parameter)
        {
        }
         */
    }
}
