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
    /// A data provider generates an enumeration of <see cref="IDataRow" />s
    /// given a collection of <see cref="DataBinding"/>s to satisfy.
    /// </summary>
    public interface IDataProvider
    {
        /// <summary>
        /// Gets an enumeration of data rows that can supply values for a given collection of bindings.
        /// </summary>
        /// <param name="bindings">The bindings that are requested</param>
        /// <returns>The enumeration of data rows</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="bindings"/> is null</exception>
        IEnumerable<IDataRow> GetRows(ICollection<DataBinding> bindings);
    }
}
