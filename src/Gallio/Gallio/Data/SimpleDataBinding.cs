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

namespace Gallio.Data
{
    /// <summary>
    /// <para>
    /// A simple minimalist implementation of a data binding.
    /// </para>
    /// </summary>
    public class SimpleDataBinding : DataBinding
    {
        private readonly Type type;
        private readonly string path;
        private readonly int? index;

        /// <summary>
        /// Creates a new data binding.
        /// </summary>
        /// <param name="valueType">The type of value to bind</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="valueType"/> is null</exception>
        public SimpleDataBinding(Type valueType)
        {
            if (valueType == null)
                throw new ArgumentNullException("valueType");

            this.type = valueType;
        }

        /// <summary>
        /// Creates a new data binding with an optional path and index.
        /// </summary>
        /// <param name="valueType">The type of value to bind</param>
        /// <param name="path">The binding path or null if none.  <seealso cref="Path"/></param>
        /// <param name="index">The binding index or null if none.  <seealso cref="Index"/></param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="valueType"/> is null</exception>
        public SimpleDataBinding(Type valueType, string path, int? index)
            : this(valueType)
        {
            this.path = path;
            this.index = index;
        }

        /// <inheritdoc />
        public override Type Type
        {
            get { return type; }
        }

        /// <inheritdoc />
        public override string Path
        {
            get { return path; }
        }

        /// <inheritdoc />
        public override int? Index
        {
            get { return index; }
        }

        /// <inheritdoc />
        public override DataBinding ReplaceIndex(int? index)
        {
            return new SimpleDataBinding(type, path, index);
        }
    }
}