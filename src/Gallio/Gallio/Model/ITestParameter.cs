// Copyright 2008 MbUnit Project - http://www.mbunit.com/
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

using Gallio.Reflection;

namespace Gallio.Model
{
    /// <summary>
    /// <para>
    /// A test parameter describes a formal parameter of a <see cref="ITest" />
    /// to which a value be bound to produce a <see cref="ITestInstance" />.
    /// </para>
    /// <para>
    /// The <see cref="ITestComponent.Name" /> property of a test parameter should be
    /// unique among the set parameters belonging to its <see cref="Owner"/>.
    /// </para>
    /// </summary>
    public interface ITestParameter : ITestComponent
    {
        /// <summary>
        /// Gets or sets the test that owns this parameter, or null if this parameter
        /// does not yet have an owner.
        /// </summary>
        ITest Owner { get; set; }

        /// <summary>
        /// Gets the type of value that must be bound to the parameter.
        /// </summary>
        ITypeInfo Type { get; }

        /// <summary>
        /// Gets the zero-based index of the parameter.  The index is used
        /// instead of the parameter name in unlabeled table-like data sources
        /// (such as row-tests and headerless CSV files) to select the column to
        /// which the parameter will be bound.
        /// </summary>
        /// <value>
        /// The index of the parameter, or 0 if not applicable.
        /// </value>
        int Index { get; }
    }
}
