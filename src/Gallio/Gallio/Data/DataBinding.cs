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

namespace Gallio.Data
{
    /// <summary>
    /// <para>
    /// A data binding object describes how a data binding is to take place.
    /// </para>
    /// <para>
    /// This class provides support for optional path-based and index-based lookup.  Subclasses
    /// may define define custom lookup rules for a bound value that are recognized by
    /// data sets.
    /// </para>
    /// </summary>
    public class DataBinding
    {
        private readonly Type valueType;
        private readonly string path;
        private readonly int? index;

        /// <summary>
        /// Creates a new data binding.
        /// </summary>
        /// <param name="valueType">The type of value to bind</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="valueType"/> is null</exception>
        public DataBinding(Type valueType)
        {
            if (valueType == null)
                throw new ArgumentNullException("valueType");

            this.valueType = valueType;
        }

        /// <summary>
        /// Creates a new data binding with an optional path and index.
        /// </summary>
        /// <param name="valueType">The type of value to bind</param>
        /// <param name="path">The binding path or null if none.  <seealso cref="Path"/></param>
        /// <param name="index">The binding index or null if none.  <seealso cref="Index"/></param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="valueType"/> is null</exception>
        public DataBinding(Type valueType, string path, int? index)
            : this(valueType)
        {
            this.path = path;
            this.index = index;
        }

        /// <summary>
        /// Gets the type of value to bind.
        /// </summary>
        public Type ValueType
        {
            get { return valueType; }
        }

        /// <summary>
        /// Gets an optional binding path that describes how to locate the bound value
        /// in the data set, such as a column name or an XPath expression.  Null if none.
        /// </summary>
        /// <remarks>
        /// The default binding path for a test parameter may be the name of the test parameter.
        /// A data set can take advantage of this convention by treating the binding path as a
        /// case-insensitive name where appropriate.
        /// </remarks>
        public string Path
        {
            get { return path; }
        }

        /// <summary>
        /// Gets an optional binding index that describes how to locate the bound value
        /// in a data set that is structured as an ordered tuple, such as the ordinal index
        /// of a cell in an array.  Null if none.
        /// </summary>
        /// <remarks>
        /// The default binding index for a test parameter may be its ordinal index
        /// in the parameter array.  A data set can take advantage of this convention by
        /// treating the binding index 
        /// </remarks>
        public int? Index
        {
            get { return index; }
        }

        /// <summary>
        /// Creates a clone of the data binding with a different index.
        /// </summary>
        /// <param name="index">The new index</param>
        /// <returns>The cloned binding</returns>
        public virtual DataBinding ReplaceIndex(int? index)
        {
            return new DataBinding(valueType, path, index);
        }

        /// <summary>
        /// Returns a debug representation of the binding as a string.
        /// </summary>
        /// <returns>The string representation</returns>
        public override string ToString()
        {
            return String.Format("Binding ValueType: {0}, Path: {1}, Index: {2}",
                valueType.FullName,
                path != null ? "'" + path + "'" : "<null>",
                index.HasValue ? index.Value.ToString() : "<null>");
        }
    }
}
