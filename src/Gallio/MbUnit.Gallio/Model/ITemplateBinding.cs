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
using MbUnit.Model.Data;

namespace MbUnit.Model
{
    /// <summary>
    /// A template binding binds a template with its actual argument values
    /// in the scope in which it was instantiated.
    /// </summary>
    public interface ITemplateBinding
    {
        /// <summary>
        /// Gets the template that has been bound.
        /// </summary>
        ITemplate Template { get; }

        /// <summary>
        /// Gets the scope in which the binding occurred.
        /// </summary>
        TemplateBindingScope Scope { get; }

        /// <summary>
        /// Gets the actual argument values used to bind the template.
        /// </summary>
        IDictionary<ITemplateParameter, IDataFactory> Arguments { get; }

        /// <summary>
        /// Builds tests from this template binding.
        /// </summary>
        /// <param name="builder">The test tree builder</param>
        /// <param name="parent">The parent test</param>
        void BuildTests(TestTreeBuilder builder, ITest parent);
    }
}
