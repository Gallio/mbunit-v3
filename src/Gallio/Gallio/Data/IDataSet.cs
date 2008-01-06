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
    /// A data set provides data rows for data binding.
    /// </summary>
    public interface IDataSet
    {
        /// <summary>
        /// <para>
        /// Returns true if the data set is dynamic and cannot be enumerated with certainty
        /// prior to its eventual use because its contents may be unavailable ahead
        /// of time, may change over time or may be expensive to obtain.
        /// </para>
        /// <para>
        /// For example, data obtained from a database should be considered dynamic.
        /// On the other hand, data obtained from declarative metadata should be considered
        /// static.
        /// </para>
        /// </summary>
        bool IsDynamic { get; }

        /// <summary>
        /// Gets the number of columns in an indexed data set.
        /// </summary>
        int ColumnCount { get; }

        /// <summary>
        /// Returns true if the data set can provide a value for the specified binding.
        /// </summary>
        /// <param name="binding">The binding</param>
        /// <returns>True if the data set can provide a value for the binding</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="binding"/> is null</exception>
        bool CanBind(DataBinding binding);

        /// <summary>
        /// Gets an enumeration of data rows in the data set.
        /// </summary>
        /// <param name="bindings">The bindings that will be accessed</param>
        /// <returns>The enumeration of data rows</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="bindings"/> is null</exception>
        IEnumerable<IDataRow> GetRows(ICollection<DataBinding> bindings);
    }
}
