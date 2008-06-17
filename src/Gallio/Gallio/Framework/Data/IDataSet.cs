// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan de Halleux
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

namespace Gallio.Framework.Data
{
    /// <summary>
    /// A data set provides data items for data binding and describes whether
    /// is supports particular bindings.
    /// </summary>
    public interface IDataSet : IDataProvider
    {
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
    }
}
