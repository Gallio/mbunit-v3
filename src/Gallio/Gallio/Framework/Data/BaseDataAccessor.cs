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
    /// A base implementation of <see cref="IDataAccessor" /> that
    /// performs argument validation.
    /// </summary>
    public abstract class BaseDataAccessor : IDataAccessor
    {
        /// <inheritdoc />
        public object GetValue(IDataItem item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            return GetValueImpl(item);
        }

        /// <summary>
        /// Internal implementation of <see cref="GetValue" /> after argument
        /// validation has been performed.
        /// </summary>
        /// <param name="item">The data item, not null</param>
        /// <returns>The value</returns>
        protected abstract object GetValueImpl(IDataItem item);
    }
}
