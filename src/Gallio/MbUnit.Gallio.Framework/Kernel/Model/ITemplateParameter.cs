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

namespace MbUnit.Framework.Kernel.Model
{
    /// <summary>
    /// <para>
    /// A template parameter describes a formal parameter of a <see cref="ITemplate" />
    /// to which a value be bound to produce a concrete instance of the template.
    /// This mechanism supports data-driven testing because each instance of a
    /// template can build tests based on the values bound to the template's parameters.
    /// </para>
    /// </summary>
    public interface ITemplateParameter : ITemplateComponent
    {
        /// <summary>
        /// Gets or sets the type of value that must be bound to the parameter.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
        Type Type { get; set; }

        /// <summary>
        /// Gets or sets the zero-based index of the parameter.  The index is used
        /// instead of the parameter name in unlabeled table-like data sources
        /// (such as row-tests and headerless CSV files) to select the column to
        /// which the parameter will be bound.
        /// </summary>
        /// <value>
        /// The default value is 0.
        /// </value>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is less than 0</exception>
        int Index { get; set; }
    }
}
