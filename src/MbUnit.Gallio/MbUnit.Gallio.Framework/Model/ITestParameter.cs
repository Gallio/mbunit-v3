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

namespace MbUnit.Framework.Model
{
    /// <summary>
    /// <para>
    /// A test parameter describes a formal parameter of a <see cref="ITestTemplate" />
    /// to which a value be bound to produce a concrete instance of the template.
    /// </para>
    /// <para>
    /// Data-driven tests are represented by <see cref="ITestTemplate"/>s with
    /// at least one parameter.
    /// </para>
    /// <para>
    /// Parameters may be grouped into parameter sets by use of a common
    /// parameter set name.  A parameter set name can be used to select
    /// </para>
    /// </summary>
    public interface ITestParameter : ITestComponent
    {
        /// <summary>
        /// Gets or sets the parameter set to which the test parameter belongs.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
        ITestParameterSet ParameterSet { get; set; }

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
        /// <remarks>
        /// <para>
        /// The index does not necessarily correspond to the sequence in which
        /// the parameter appears in its <see cref="ParameterSet" />.
        /// </para>
        /// <para>
        /// Exotic data sources that do not bind by name or by index may use metadata
        /// associated with the parameter to specify how data binding will occur.
        /// For example, metadata containing an XPath expression could be used by XML data sources.
        /// </para>
        /// </remarks>
        /// <value>
        /// The default value is 0.
        /// </value>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is less than 0</exception>
        int Index { get; set; }
    }
}
