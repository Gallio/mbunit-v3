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
using MbUnit.Framework.Kernel.Model;

namespace MbUnit.Framework.Kernel.DataBinding
{
    /// <summary>
    /// An array data row stores its factories as an array and allows
    /// its metadata to be updated.
    /// </summary>
    public class ArrayDataRow : IDataRow
    {
        private readonly MetadataMap metadata;
        private readonly IDataFactory[] factories;

        /// <summary>
        /// Creates an array data row.
        /// </summary>
        /// <param name="factories">The row's data factories</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="factories"/> is null</exception>
        public ArrayDataRow(IDataFactory[] factories)
        {
            if (factories == null)
                throw new ArgumentNullException("factories");

            this.factories = factories;
            metadata = new MetadataMap();
        }

        /// <inheritdoc />
        public MetadataMap Metadata
        {
            get { return metadata; }
        }

        /// <inheritdoc />
        public IList<IDataFactory> Factories
        {
            get { return factories; }
        }
    }
}
