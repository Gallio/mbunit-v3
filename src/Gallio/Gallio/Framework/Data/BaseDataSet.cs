// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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

namespace Gallio.Framework.Data
{
    /// <summary>
    /// Abstract base class for <see cref="IDataSet" /> that validates
    /// input arguments before passing them on to the implementation.
    /// </summary>
    public abstract class BaseDataSet : IDataSet
    {
        /// <inheritdoc />
        public abstract int ColumnCount { get; }

        /// <inheritdoc />
        public bool CanBind(DataBinding binding)
        {
            if (binding == null)
                throw new ArgumentNullException("binding");

            return CanBindImpl(binding);
        }

        /// <inheritdoc />
        public IEnumerable<IDataRow> GetRows(ICollection<DataBinding> bindings, bool includeDynamicRows)
        {
            if (bindings == null)
                throw new ArgumentNullException("bindings");

            return GetRowsImpl(bindings, includeDynamicRows);
        }

        /// <summary>
        /// Determines whether a binding can be satisfied by the data set.
        /// </summary>
        /// <param name="binding">The data binding, not null</param>
        /// <returns>True if the binding can be satisfied</returns>
        protected abstract bool CanBindImpl(DataBinding binding);

        /// <summary>
        /// Enumerates the rows in the data set.
        /// </summary>
        /// <param name="bindings">The data bindings, not null</param>
        /// <param name="includeDynamicRows">If true, includes dynamic rows</param>
        /// <returns>The enumeration of rows in the combined data set</returns>
        protected abstract IEnumerable<IDataRow> GetRowsImpl(ICollection<DataBinding> bindings, bool includeDynamicRows);
    }
}
