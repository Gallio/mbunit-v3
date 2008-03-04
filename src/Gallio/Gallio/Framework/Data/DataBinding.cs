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

namespace Gallio.Framework.Data
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
    /// <seealso cref="SimpleDataBinding"/>
    public abstract class DataBinding
    {
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
        public abstract int? Index { get; }

        /// <summary>
        /// Gets an optional binding path that describes how to locate the bound value
        /// in the data set, such as a column name or an XPath expression.  Null if none.
        /// </summary>
        /// <remarks>
        /// The default binding path for a test parameter may be the name of the test parameter.
        /// A data set can take advantage of this convention by treating the binding path as a
        /// case-insensitive name where appropriate.
        /// </remarks>
        public abstract string Path { get; }

        /// <summary>
        /// Creates a clone of the data binding with a different index.
        /// </summary>
        /// <param name="index">The new index</param>
        /// <returns>The cloned binding</returns>
        public abstract DataBinding ReplaceIndex(int? index);

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            DataBinding other = obj as DataBinding;
            return other != null
                && GetType() == other.GetType()
                && Index == other.Index
                && Path == other.Path;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return (Path != null ? Path.GetHashCode() : 0)
                ^ (Index.HasValue ? Index.Value : -1);
        }

        /// <summary>
        /// Returns a debug representation of the binding as a string.
        /// </summary>
        /// <returns>The string representation</returns>
        public override string ToString()
        {
            return String.Format("Binding Index: {0}, Path: {1}",
                Index.HasValue ? Index.Value.ToString() : "<null>",
                Path != null ? "'" + Path + "'" : "<null>");
        }
    }
}
