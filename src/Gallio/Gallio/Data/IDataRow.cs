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

using System;
using System.Collections.Generic;

namespace Gallio.Data
{
    /// <summary>
    /// <para>
    /// A data row provides a means for obtaining values or descriptions of values
    /// associated with data bindings.  Each row may include metadata to describe
    /// the purpose of the row.
    /// </para>
    /// </summary>
    public interface IDataRow
    {
        /// <summary>
        /// <para>
        /// Gets the metadata key/value pairs associated with the data row, if any.
        /// </para>
        /// <para>
        /// For example, the metadata may contain a description that serves
        /// as documentation of the contents of the data row or of the test
        /// scenario that is exercised by the contents of the data row.
        /// This metadata may be injected into test instances created with
        /// the contents of this data row.
        /// </para>
        /// </summary>
        IEnumerable<KeyValuePair<string, string>> GetMetadata();

        /// <summary>
        /// Gets the value of the specified binding.
        /// </summary>
        /// <remarks>
        /// The implementation may use the <see cref="DataBinding.Type" /> property
        /// as a hint when retrieving source data, but it is not required to produce an
        /// object of that type.  The data binding infrastructure will take care of
        /// any necessary conversions.
        /// </remarks>
        /// <param name="binding">The data binding, never null</param>
        /// <returns>The value</returns>
        /// <exception cref="DataBindingException">Thrown if the <paramref name="binding"/>
        /// cannot be resolved or if its value cannot be obtained</exception>
        object GetValue(DataBinding binding);
    }
}
