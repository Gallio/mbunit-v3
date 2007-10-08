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

namespace MbUnit.Model.Data
{
    /// <summary>
    /// A data binder provides services for performing data binding.
    /// </summary>
    public interface IDataBinder
    {
        /// <summary>
        /// Gets the data binder's converter.
        /// </summary>
        IConverter Converter { get; }

        /// <summary>
        /// Creates a data binding context.
        /// </summary>
        /// <returns>The context</returns>
        IDataBindingContext CreateContext();

        /// <summary>
        /// Obtains an enumeration of data rows that satisfy the request.
        /// </summary>
        /// <param name="set">The data binding request</param>
        /// <param name="resolver">The data source resolver</param>
        /// <returns>The enumeration of data rows</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="set"/>
        /// or <paramref name="resolver"/> is null</exception>
        IEnumerable<IDataRow> Bind(DataBindingSet set, IDataSourceResolver resolver);
    }
}
